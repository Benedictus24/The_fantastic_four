using Microsoft.EntityFrameworkCore.Design;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
namespace MR_Backend.Models
{
	public class User
	{
		[Key]
		public int UserId { get; set; }
		[Required]
		public string Name { get; set; }
		[Required]
		public string Surname { get; set; }
		[Required]
		public string Email { get; set; }
		[Required]
		public string Password { get; set; }
		public string PhoneNumber { get; set; }
		public DateTime Birthday { get; set; }
		public string? Token { get; set; }

		public string? RefreshToken { get; set; }
		public DateTime RefreshTokenExpiryTime { get; set; }
		public string? ResetPasswordToken { get; set; }
		public DateTime ResetPasswordTokenExpiry { get; set; }


	}
}
