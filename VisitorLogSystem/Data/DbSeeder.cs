using System;
using System.Drawing;
using System.Linq;
using System.Threading;
using VisitorLogSystem.Models;
using static System.Net.Mime.MediaTypeNames;

namespace VisitorLogSystem.Data
{
    
    /// Database Seeder - Creates initial admin user
    /// This runs when the application starts to ensure we have at least one admin account
    
    public static class DbSeeder
    {
        
        /// Seeds the database with default admin user
        
        /// <param name="context">Database context</param>
        public static void SeedDatabase(ApplicationDbContext context)
        {
            
            context.Database.EnsureCreated();

            // Check if we already have users (avoid creating duplicates)
            if (context.Users.Any())
            {
               
                Console.WriteLine("ℹ️ Database already has users. Skipping seed.");
                return;
            }

            // Create default admin user
            // ⭐ Using BCrypt to hash password (same as AuthService!)
            var adminUser = new User
            {
                Username = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),  // ⭐ BCrypt!
                Role = "Admin",
                CreatedAt = DateTime.Now
            };

            // Add to database
            context.Users.Add(adminUser);
            context.SaveChanges();

            Console.WriteLine("✅ Database seeded successfully!");
            Console.WriteLine("📝 Default admin user created:");
            Console.WriteLine("   Username: admin");
            Console.WriteLine("   Password: admin123");
            Console.WriteLine("   Role: Admin");
        }
    }
}
