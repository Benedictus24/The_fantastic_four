using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MR_Backend.Models
{
	public class AppDbContext:DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
		{
		}
		public DbSet<Admin> Admin { get; set; }
		public DbSet<General_User> General_User { get; set; }
		public DbSet<Hours_Worked>Hours_Worked { get; set; }
		public DbSet<Meeting> Meeting { get; set; }	
		public DbSet<Role> Role { get; set; }	
		public DbSet<User> User { get; set; }
		public DbSet<User_Role> User_Role { get; set; }


	}
}
