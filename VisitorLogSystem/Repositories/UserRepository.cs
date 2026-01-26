using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using VisitorLogSystem.Data;
using VisitorLogSystem.Interfaces;
using VisitorLogSystem.Models;

namespace VisitorLogSystem.Repositories
{
    
    /// RESPONSIBILITY: ONLY database access, NO business logic
    
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

      
        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        
        /// Find user by username
        
        public async Task<User?> GetUserByUsernameAsync(string username)
        {
          
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        
        /// Find user by ID
        
        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id);
        }

      
        /// Create new user account
      
        public async Task<User> CreateUserAsync(User user)
        {
            
            user.CreatedAt = DateTime.Now;

           
            _context.Users.Add(user);

           
            await _context.SaveChangesAsync();

            
            return user;
        }

       
        /// Update existing user
      
        public async Task<User?> UpdateAsync(User user)
        {
            try
            {
             
                _context.Entry(user).State = EntityState.Modified;

               
                await _context.SaveChangesAsync();

                return user;
            }
            catch (DbUpdateConcurrencyException)
            {
                
                return null;
            }
        }

      
        /// Delete user by ID
       
        public async Task<bool> DeleteAsync(int id)
        {
          
            var user = await GetUserByIdAsync(id);

            
            if (user == null)
            {
                return false; 
            }

            
            _context.Users.Remove(user);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}