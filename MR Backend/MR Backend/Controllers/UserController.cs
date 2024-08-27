using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MR_Backend.Models;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Text;
using Microsoft.Extensions.Configuration;
using MR_Backend.models.Dto;
using MR_Backend.Helpers;
using MR_Backend.EmailService;


namespace MR_Backend.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UserController : Controller
	{
		private readonly IConfiguration _configuration;
		private readonly AppDbContext _authContext;
		private readonly IRepository _repository;
		private readonly IEmailService _emailService;
		public UserController(AppDbContext context, IRepository repository, IConfiguration configuration, IEmailService emailService)
		{
			_authContext = context;
			_configuration = configuration;
			_repository = repository;
			_emailService = emailService;
		}

		//**************************************************************** Get Users **********************************************************
		[HttpGet]
		public async Task<ActionResult<User>> GetAllUsers()
		{
			return Ok(await _authContext.User.ToListAsync());
		}

		//**************************************************************** Get Users **********************************************************

		[HttpGet]
		[Route("GetUsers")]
		public async Task<IActionResult> GetUsers()
		{
			try
			{
				var results = await _repository.GetUsersAsync();
				// Transform the results
				dynamic users = results.Select(e => new
				{
					e.UserId,
					e.Name,
					e.Surname,
					e.PhoneNumber,
					e.Email,
					e.Birthday
				});


				return Ok(users);
			}
			catch (Exception ex)
			{
				// Log the exception
				Debug.WriteLine($"Exception: {ex}");
				return StatusCode(500, "Internal Server Error. Please contact support.");
			}
		}

		//***************************************************** Send reset email **************************************************
		[HttpPost]
		[Route("send-reset-email/{email}")]
		public async Task<IActionResult> SendEmail(string email)
		{
			var user = await _authContext.User.FirstOrDefaultAsync(a => a.Email == email);
			if (user is null)
			{
				return NotFound(new
				{
					StatusCode = 404,
					Message = "Email does not exist"
				});
			}
			var tokenBytes = RandomNumberGenerator.GetBytes(64);
			var emailToken = Convert.ToBase64String(tokenBytes);
			user.ResetPasswordToken = emailToken;
			user.ResetPasswordTokenExpiry = DateTime.Now.AddMinutes(15);
			string from = _configuration["EmailSetting:From"];
			var emailModel = new EmailModel(email, "Reset Password!!", EmailBody.EmailStringBody(email, emailToken));
			_emailService.SendEmail(emailModel);
			_authContext.Entry(user).State = EntityState.Modified;
			await _authContext.SaveChangesAsync();
			return Ok(new
			{
				StatusCode = 200,
				Message = "Email successfully sent!"
			});
		}

		//*********************************************************************** Reset Password ******************************************************************


		[HttpPost]
		[Route("Reset-password")]
		public async Task<IActionResult> ResetPassword(ResetPasswordDto resetPasswordDto)
		{
			var newToken = resetPasswordDto.EmailToken.Replace(" ", "+");

				// User not found in the Users table
				var user = await _authContext.User.FirstOrDefaultAsync(a => a.Email == resetPasswordDto.Email);
				if (user == null)
				{
					return NotFound(new
					{
						StatusCode = 404,
						Message = "User does not exist"
					});
				}

			var tokenCode = user.ResetPasswordToken;
			DateTime emailTokenExpiry = user.ResetPasswordTokenExpiry;

			if (tokenCode != resetPasswordDto.EmailToken || emailTokenExpiry < DateTime.Now)
			{
				return NotFound(new
				{
					StatusCode = 400,
					Message = "Invalid Reset link"
				});
			}

			user.Password = PasswordHasher.HashPassword(resetPasswordDto.NewPassword);
			_authContext.Entry(user).State = EntityState.Modified;
			await _authContext.SaveChangesAsync();

			return Ok(new
			{
				StatusCode = 200,
				Message = "Password successfully reset"
			});
		}


		//*******************************************************************Change Password ******************************************************************
		[HttpPost("ChangePassword")]
		/*	[Authorize] */// Make sure the user is authenticated
		public async Task<IActionResult> ChangePassword([FromBody] ChangePassword model)
		{
			try
			{
				// Retrieve the user based on the authenticated user or a token
				var user = await _authContext.User.FirstOrDefaultAsync(u => u.Email == User.Identity.Name);

				// Check if the user exists
				if (user == null)
				{
					return NotFound("User not found.");
				}

				// Check if the old password matches the stored password
				if (!PasswordHasher.VerifyPassword(model.OldPassword, user.Password))
				{
					return BadRequest("Old password is incorrect.");
				}

				// Check if the new password is different from the old password
				if (model.NewPassword == model.OldPassword)
				{
					return BadRequest("New password must be different from the old password.");
				}

				// Check if the new password and confirm password match
				if (model.NewPassword != model.ConfirmPassword)
				{
					return BadRequest("New password and confirm password do not match.");
				}

				// Update the password
				user.Password = PasswordHasher.HashPassword(model.NewPassword);
				await _authContext.SaveChangesAsync();

				return Ok(new { Message = "Password changed successfully." });
			}
			catch (Exception ex)
			{
				// Return a detailed error message to the client
				return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An error occurred while processing your request.", Details = ex.Message });
			}

		}


		//****************************************************************************** Getting User using id ***********************************************************
		[HttpGet]
		[Route("Profile/{UserId}")]
		public async Task<ActionResult> GetUser(int UserId)
		{
			try
			{
				var users = await _repository.ViewProfileAsync(UserId);
				if (users == null) return NotFound("User does not exist.");
				return Ok(users);
			}
			catch (Exception)
			{
				return StatusCode(500, "Enter some error message");
			}

		}


		//********************************************************************************** Getting User using id ******************************************************
		[HttpGet]
		[Route("ProfileIn")]
		public async Task<ActionResult<User>> ViewProfiles()
		{
			return Ok(await _authContext.User.ToListAsync());
		}

		//********************************************************************************** Edit User *************************************************************
		[HttpPut]
		[Route("EditUser/{UserId}")]
		public async Task<ActionResult<User>> EditUser(int UserId, User ftvm)
		{
			try
			{
				var existingUser = await _repository.ViewProfileAsync(UserId);

				// fix error message
				if (existingUser == null) return NotFound($"The user does not exist");

				existingUser.Name = ftvm.Name;
				existingUser.Email = ftvm.Email;
				existingUser.Surname = ftvm.Surname;
				existingUser.Birthday = ftvm.Birthday;
				existingUser.PhoneNumber = ftvm.PhoneNumber;

				if (await _repository.SaveChangesAsync())
				{
					return Ok(existingUser);
				}
			}
			catch (Exception)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error. Please contact support.");
			}
			return BadRequest("Your request is invalid");
		}


		//*************************************************************************** Delete User *********************************************************************
		[HttpDelete]
		[Route("DeleteUser/{UserId}")]
		public async Task<IActionResult> DeleteUser(int UserId)
		{
			try
			{
				var existingUser = await _repository.ViewProfileAsync(UserId);

				// fix error message
				if (existingUser == null) return NotFound($"The user does not exist");

				_repository.Delete(existingUser);

				if (await _repository.SaveChangesAsync())
				{
					return Ok(existingUser);
				}
			}
			catch (Exception)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error. Please contact support.");
			}
			return BadRequest("Your request is invalid");
		}


		//*************************************************************************** EditUserRole ***************************************************************************
		[HttpPut]
		[Route("EditUserRole")]
		public async Task<IActionResult> EditUserRole(int userId, int newRoleId)
		{
			// TODO: Add authentication and authorization check here to ensure only admins can access this endpoint

			var user = await _authContext.User.FindAsync(userId);
			if (user == null)
				return NotFound(new { Message = "User not found" });

			var newRole = await _authContext.Role.FindAsync(newRoleId);
			if (newRole == null)
				return NotFound(new { Message = "Role not found" });

			var existingUserRole = await _authContext.User_Role
				.FirstOrDefaultAsync(ur => ur.UserId == userId);

			if (existingUserRole != null)
			{
				existingUserRole.RoleId = newRoleId;
			}
			else
			{
				var newUserRole = new User_Role
				{
					UserId = userId,
					RoleId = newRoleId
				};
				await _authContext.User_Role.AddAsync(newUserRole);
			}

			await _authContext.SaveChangesAsync();

			return Ok(new
			{
				Status = 200,
				Message = "User role updated successfully!"
			});
		}

	}
}
