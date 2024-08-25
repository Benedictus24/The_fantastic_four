﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MR_Backend.Models
{
	public class Hours_Worked
	{
		[Key]
		public int WorkId { get; set; }
		[ForeignKey("GeneralUser")]
		public int GeneralUsrId { get; set; }
		public virtual General_User General_User { get; set; }
		public DateTime Time_In { get; set; }
		public DateTime Time_Out { get; set; }
	}
}