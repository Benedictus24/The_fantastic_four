using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MR_Backend.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Text;
using Microsoft.EntityFrameworkCore;
using MR_Backend.models.Dto;
using MR_Backend.Helpers;

namespace MR_Backend.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : Controller
	{
		private readonly IConfiguration _configuration;
		private readonly AppDbContext _authContext;
		private readonly IRepository _repository;
		public AuthController(AppDbContext context, IRepository repository, IConfiguration configuration)
		{
			_authContext = context;
			_configuration = configuration;
			_repository = repository;
		}

		//********************************************************************************* Authenticate ******************************************************************
		[HttpPost("Authenticate")]
		public async Task<IActionResult> Authenticate([FromBody] User userObj)
		{
			if (userObj == null)
				return BadRequest();

			var user = await _authContext.User
				.FirstOrDefaultAsync(x => x.Email == userObj.Email);

			if (user == null)
				return NotFound(new { Message = "User not found!" });

			if (!PasswordHasher.VerifyPassword(userObj.Password, user.Password))
			{
				return BadRequest(new { Message = "Password is Incorrect" });
			}

			user.Token = CreateJwt(user);
			var newAccessToken = user.Token;
			var newRefreshToken = CreateRefreshToken();
			user.RefreshToken = newRefreshToken;
			user.RefreshTokenExpiryTime = DateTime.Now.AddDays(15);
			await _authContext.SaveChangesAsync();

			return Ok(new TokenApiDto()
			{
				AccessToken = newAccessToken,
				RefreshToken = newRefreshToken
			});
		}

		//************************************************************************* Register *******************************************************************
		[HttpPost]
		[Route("Register")]
		public async Task<IActionResult> Register([FromBody] User userObj)
		{
			if (userObj == null)
				return BadRequest();

			// Check email
			if (await CheckEmailExistAsync(userObj.Email))
				return BadRequest(new { Message = "Email already exists" });

			var passMessage = CheckPasswordStrength(userObj.Password);
			if (!string.IsNullOrEmpty(passMessage))
				return BadRequest(new { Message = passMessage.ToString() });

			userObj.Password = PasswordHasher.HashPassword(userObj.Password);
			userObj.Token = "";

			// Start a transaction
			using var transaction = await _authContext.Database.BeginTransactionAsync();

			try
			{
				// Add user
				await _authContext.User.AddAsync(userObj);
				await _authContext.SaveChangesAsync();

				// Assign "General User" role by default during registration
				var generalUserRole = await _authContext.Role
					.FirstOrDefaultAsync(r => r.Description == "General User");

				if (generalUserRole == null)
				{
					// Create "General User" role if it doesn't exist
					generalUserRole = new Role { Description = "General User" };
					_authContext.Role.Add(generalUserRole);
					await _authContext.SaveChangesAsync();
				}

				// Assign "General User" role to the new user
				var userRole = new User_Role
				{
					UserId = userObj.UserId,
					RoleId = generalUserRole.RoleId
				};
				await _authContext.User_Role.AddAsync(userRole);

				// Create General_User entry
				var generalUser = new General_User
				{
					UserId = userObj.UserId,
					JobDescription = "New User" // Default job description
				};
				await _authContext.General_User.AddAsync(generalUser);

				await _authContext.SaveChangesAsync();

				// Commit transaction
				await transaction.CommitAsync();

				return Ok(new
				{
					Status = 200,
					Message = "User registered successfully!"
				});
			}
			catch
			{
				// Rollback transaction if any error occurs
				await transaction.RollbackAsync();
				return StatusCode(500, new { Message = "An error occurred while registering the user." });
			}
		}
		//**************************************************************************** Email valiadtion *******************************************************************************
		private Task<bool> CheckEmailExistAsync(string? email)
			=> _authContext.User.AnyAsync(x => x.Email == email);


		//**************************************************************************** Password validation *******************************************************************************
		private static string CheckPasswordStrength(string pass)
		{
			StringBuilder sb = new StringBuilder();
			if (pass.Length < 9)
				sb.Append("Minimum password length should be 8" + Environment.NewLine);
			if (!(Regex.IsMatch(pass, "[a-z]") && Regex.IsMatch(pass, "[A-Z]") && Regex.IsMatch(pass, "[0-9]")))
				sb.Append("Password should be AlphaNumeric" + Environment.NewLine);
			if (!Regex.IsMatch(pass, "[<,>,@,!,#,$,%,^,&,*,(,),_,+,\\[,\\],{,},?,:,;,|,',\\,.,/,~,`,-,=]"))
				sb.Append("Password should contain special charcter" + Environment.NewLine);
			return sb.ToString();
		}


		//**************************************************************************** Create JWT *******************************************************************************
		private string CreateJwt(User user)
		{
			var jwtTokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.ASCII.GetBytes("veryverysceretandsecurekey123456789012.....");
			var identity = new ClaimsIdentity(new Claim[]
			{
				new Claim(ClaimTypes.NameIdentifier, $"{user.UserId}"),
				new Claim(ClaimTypes.Name,$"{user.Name}"),
				new Claim(ClaimTypes.GivenName,$"{user.Birthday}"),
				new Claim(ClaimTypes.Surname,$"{user.Surname}"),
				new Claim(ClaimTypes.WindowsAccountName,$"{user.PhoneNumber}"),
				new Claim(ClaimTypes.Email,$"{user.Email}")

			});

			var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = identity,
				Expires = DateTime.Now.AddMinutes(15),
				SigningCredentials = credentials
			};
			var token = jwtTokenHandler.CreateToken(tokenDescriptor);
			return jwtTokenHandler.WriteToken(token);
		}


		//**************************************************************************** Create Refresh Token *******************************************************************************
		private string CreateRefreshToken()
		{
			var tokenBytes = RandomNumberGenerator.GetBytes(64);
			var refreshToken = Convert.ToBase64String(tokenBytes);

			var tokenInUser = _authContext.User
				.Any(a => a.RefreshToken == refreshToken);
			if (tokenInUser)
			{
				return CreateRefreshToken();
			}
			return refreshToken;
		}


		//**************************************************************************** Expired Token *******************************************************************************
		private ClaimsPrincipal GetPrincipleFromExpiredToken(string token)
		{
			var key = Encoding.ASCII.GetBytes("veryverysceret.....");
			var tokenValidationParameters = new TokenValidationParameters
			{
				ValidateAudience = false,
				ValidateIssuer = false,
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = new SymmetricSecurityKey(key),
				ValidateLifetime = false
			};
			var tokenHandler = new JwtSecurityTokenHandler();
			SecurityToken securityToken;
			var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
			var jwtSecurityToken = securityToken as JwtSecurityToken;
			if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
				throw new SecurityTokenException("This is Invalid Token");
			return principal;

		}

		//************************************************************** Refresh ************************************************************8
		[HttpPost]
		[Route("Refresh")]
		public async Task<IActionResult> Refresh([FromBody] TokenApiDto tokenApiDto)
		{
			if (tokenApiDto is null)
				return BadRequest("Invalid Client Request");
			string accessToken = tokenApiDto.AccessToken;
			string refreshToken = tokenApiDto.RefreshToken;
			var principal = GetPrincipleFromExpiredToken(accessToken);
			var username = principal.Identity.Name;
			var user = await _authContext.User.FirstOrDefaultAsync(u => u.Email == username);
			if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
				return BadRequest("Invalid Request");
			var newAccessToken = CreateJwt(user);
			var newRefreshToken = CreateRefreshToken();
			user.RefreshToken = newRefreshToken;
			await _authContext.SaveChangesAsync();
			return Ok(new TokenApiDto()
			{
				AccessToken = newAccessToken,
				RefreshToken = newRefreshToken,
			});
		}

	}
}
