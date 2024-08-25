using System.ComponentModel.DataAnnotations;

namespace MR_Backend.Models
{
	public class Role
	{
		[Key]
		public int RoleId { get; set; }
		public string Description { get; set; }
	}
}
