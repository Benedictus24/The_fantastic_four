using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MR_Backend.Models;

namespace MR_Backend.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class Hours_WorkedController : ControllerBase
	{
		private readonly IRepository _repository;

		public Hours_WorkedController(IRepository repository)
		{
			_repository = repository;
		}

	
		[HttpGet("user/{userId}")]
		public async Task<ActionResult<IEnumerable<Hours_Worked>>> GetHoursWorkedByUser(int userId)
		{
			var hoursWorked = await _repository.GetByUserIdAsync(userId);
			return Ok(hoursWorked);
		}


		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateHoursWorked(int id, Hours_Worked hoursWorked)
		{
			if (id != hoursWorked.WorkId)
			{
				return BadRequest();
			}

			await _repository.UpdateAsync(hoursWorked);
			return NoContent();
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteHoursWorked(int id)
		{
			await _repository.DeleteAsync(id);
			return NoContent();
		}
	}
}