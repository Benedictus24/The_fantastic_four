using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MR_Backend.Models
{
	public class Hours_Worked
	{
		[Key]
		public int WorkId { get; set; }

		[ForeignKey("General_User")]
		public int GeneralUserId { get; set; }
		public virtual General_User General_User { get; set; }
		public DateTime Time_In { get; set; }
		public DateTime Time_Out { get; set; }
	}
}
