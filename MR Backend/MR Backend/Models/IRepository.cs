namespace MR_Backend.Models
{
	public interface IRepository
	{
		//*********************** Save, Add , Delete and Update Methods *************************************************************************** 
		void Add<T>(T entity) where T : class;
		void Delete<T>(T entity) where T : class;
		void Update<T>(T entity) where T : class;
		Task<bool> SaveChangesAsync();

		// *******************************  User *************************************************************************** 
		Task<User[]> ViewProfileAsync();
		Task<User> ViewProfileAsync(int UserId);
		Task<User[]> GetUsersAsync();

	}
}
