using Microsoft.EntityFrameworkCore;

namespace MR_Backend.Models
{
	public class Repository : IRepository
	{

		private readonly AppDbContext _appDbContext;
		public Repository(AppDbContext appDbContext)
		{
			_appDbContext = appDbContext;

		}

		//********************* Save, Add , Delete and Update Methods *************************************************************************** 
		public async Task<bool> SaveChangesAsync()
		{
			return await _appDbContext.SaveChangesAsync() > 0;
		}
		public void Add<T>(T entity) where T : class
		{
			_appDbContext.Add(entity);
		}
		public void Delete<T>(T entity) where T : class
		{
			_appDbContext.Remove(entity);
		}

		public void Update<T>(T entity) where T : class
		{
			_appDbContext.Update(entity);
		}
		//***************************************************************************  User *************************************************************************** 
		public async Task<User[]> ViewProfileAsync()
		{
			IQueryable<User> query = _appDbContext.User;
			return await query.ToArrayAsync();
		}

		public async Task<User> ViewProfileAsync(int UserId)
		{
			IQueryable<User> query = _appDbContext.User.Where(u => u.UserId == UserId);
			return await query.FirstOrDefaultAsync();
		}

		public async Task<User[]> GetUsersAsync()
		{
			IQueryable<User> query = _appDbContext.User;
			return await query.ToArrayAsync();
		}

		public async Task<User> GetUserProfile(int UserId)
		{
			// Fetch the user profile details from the database or any other data source
			var user = await _appDbContext.User.FindAsync(UserId);

			// Map the user entity to a DTO or view model object
			var userProfile = new User
			{
				Name = user.Name,
				Surname = user.Surname,
				PhoneNumber = user.PhoneNumber,
				Birthday = user.Birthday,
				Email = user.Email
			};

			return userProfile;
		}
		//----------------------------------------------- TRACKING HOURS----------------------------------------------------------


		public async Task<IEnumerable<Hours_Worked>> GetByUserIdAsync(int userId)
		{
			return await _appDbContext.Hours_Worked
				.Where(h => h.GeneralUserId == userId)
				.Include(h => h.General_User)
				.ToListAsync();
		}
		public async Task<Hours_Worked> CreateAsync(Hours_Worked newSession)
		{
			_appDbContext.Hours_Worked.Add(newSession);
			await _appDbContext.SaveChangesAsync();
			return newSession;
		}

		public async Task<Hours_Worked> GetByIdAsync(int workId)
		{
			return await _appDbContext.Hours_Worked
				.Include(h => h.General_User)
				.FirstOrDefaultAsync(h => h.WorkId == workId);
		}

		public async Task UpdateAsync(Hours_Worked hoursWorked)
		{
			_appDbContext.Entry(hoursWorked).State = EntityState.Modified;
			await _appDbContext.SaveChangesAsync();
		}

		public async Task DeleteAsync(int id)
		{
			var hoursWorked = await _appDbContext.Hours_Worked.FindAsync(id);
			if (hoursWorked != null)
			{
				_appDbContext.Hours_Worked.Remove(hoursWorked);
				await _appDbContext.SaveChangesAsync();
			}
		}
	}
}

