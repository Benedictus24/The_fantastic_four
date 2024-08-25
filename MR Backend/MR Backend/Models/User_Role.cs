using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace MR_Backend.Models
{
	public class User_Role
	{
		[Key]
		public int Id { get; set; }
		[ForeignKey("User")]
		public int UserId { get; set; }
		public virtual User User { get; set; }
		[ForeignKey("Role")]
		public int RoleId { get; set; }
		public virtual Role Role { get; set; }	

	}
}
