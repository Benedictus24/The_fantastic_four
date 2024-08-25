using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MR_Backend.Models
{
	public class Meeting
	{
		[Key]
		public int MeetingId { get; set; }
		[ForeignKey("General_User")]
		public int GeneralUserId { get; set; }
		public virtual General_User General_User { get; set; }
		public DateTime Time_Start { get; set; }
		public DateTime Time_End { get; set; }
		public string Description { get; set; }
	}
}
