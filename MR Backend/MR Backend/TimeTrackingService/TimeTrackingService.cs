using System;
using System.Threading.Tasks;
using MR_Backend.Models;

namespace MR_Backend.Services
{
	public interface ITimeTrackingService
	{
		Task<Hours_Worked> StartTracking(int userId);
		Task<Hours_Worked> StopTracking(int workId);
	}

	public class TimeTrackingService : ITimeTrackingService
	{
		private readonly IRepository _repository;

		public TimeTrackingService(IRepository repository)
		{
			_repository = repository;
		}
		public async Task<Hours_Worked> StartTracking(int userId)
		{
			int adjustedUserId = userId > 1 ? userId - 1 : userId;

			var newSession = new Hours_Worked
			{
				GeneralUserId = adjustedUserId,
				Time_In = DateTime.UtcNow
			};

			return await _repository.CreateAsync(newSession);
		}


		public async Task<Hours_Worked> StopTracking(int workId)
		{
			var session = await _repository.GetByIdAsync(workId);
			if (session == null)
			{
				throw new ArgumentException("Invalid work session id", nameof(workId));
			}

			session.Time_Out = DateTime.UtcNow;
			await _repository.UpdateAsync(session);
			return session;
		}
	}
}