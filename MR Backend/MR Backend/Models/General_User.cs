using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MR_Backend.Models
{
	public class General_User
	{
		[Key]
		public int GeneralUserId { get; set; }
		[ForeignKey("User")]
		public int UserId { get; set; }
		public virtual User User { get; set; }	
		public string JobDescription	{ get; set; }

	}
}
