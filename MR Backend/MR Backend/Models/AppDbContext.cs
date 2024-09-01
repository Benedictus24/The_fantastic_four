using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MR_Backend.Helpers;

namespace MR_Backend.Models
{
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
		{
		}
		public DbSet<Admin> Admin { get; set; }
		public DbSet<General_User> General_User { get; set; }
		public DbSet<Hours_Worked> Hours_Worked { get; set; }
		public DbSet<Meeting> Meeting { get; set; }
		public DbSet<Role> Role { get; set; }
		public DbSet<User> User { get; set; }
		public DbSet<User_Role> User_Role { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			// Seed Admin role
			modelBuilder.Entity<Role>().HasData(new Role
			{
				RoleId = 1,
				Description = "Admin"
			});

			// Seed Admin user
			var adminUserId = 1;
			var adminRoleId = 1;
			var adminPassword = PasswordHasher.HashPassword("Admin123!"); // Make sure to use the same hashing mechanism

			modelBuilder.Entity<User>().HasData(new User
			{
				UserId = adminUserId,
				Name = "Admin",
				Surname = "User",
				Email = "admin@example.com",
				Password = adminPassword,
				PhoneNumber = "1234567890",
				Birthday = new DateTime(1980, 1, 1),
				Token = null,
				RefreshToken = null,
				RefreshTokenExpiryTime = DateTime.MinValue,
				ResetPasswordToken = null,
				ResetPasswordTokenExpiry = DateTime.MinValue
			});

			// Assign Admin role to Admin user
			modelBuilder.Entity<User_Role>().HasData(new User_Role
			{
				Id = 1,
				UserId = adminUserId,
				RoleId = adminRoleId
			});
		}
	}
}
