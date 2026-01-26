using System.Threading.Tasks;
using VisitorLogSystem.Models;

namespace VisitorLogSystem.Interfaces
{
   
    /// RESPONSIBILITY: Database operations for User table
  
    public interface IUserRepository
    {
       
        /// Find user by username
     
        Task<User?> GetUserByUsernameAsync(string username);

        
        /// Find user by ID
       
        Task<User?> GetUserByIdAsync(int id);

      
        /// Create new user account
       
        Task<User> CreateUserAsync(User user);


       
        /// Update existing user
      
        Task<User?> UpdateAsync(User user);

        
        /// Delete user by ID
        
        Task<bool> DeleteAsync(int id);
    }
}