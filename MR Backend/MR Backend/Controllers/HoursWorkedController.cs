using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MR_Backend.Models;
using MR_Backend.Services;

namespace MR_Backend.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class Hours_WorkedController : ControllerBase
	{
		private readonly IRepository _repository;
		private readonly ITimeTrackingService _timeTrackingService;

		public Hours_WorkedController(IRepository repository, ITimeTrackingService timeTrackingService)
		{
			_repository = repository;
			_timeTrackingService = timeTrackingService;
		}

		[HttpGet("user/{userId}")]
		public async Task<ActionResult<IEnumerable<Hours_Worked>>> GetHoursWorkedByUser(int userId)
		{
			var hoursWorked = await _repository.GetByUserIdAsync(userId);
			return Ok(hoursWorked);
		}

		[HttpPost("start")]
		public async Task<ActionResult<Hours_Worked>> StartWork([FromBody] StartWorkRequest request)
		{
			var startedSession = await _timeTrackingService.StartTracking(request.UserId);
			return Ok(startedSession);
		}

		[HttpPost("stop/{workId}")]
		public async Task<ActionResult<Hours_Worked>> StopWork(int workId)
		{
			var stoppedSession = await _timeTrackingService.StopTracking(workId);
			return Ok(stoppedSession);
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

	public class StartWorkRequest
	{
		public int UserId { get; set; }
	}
}