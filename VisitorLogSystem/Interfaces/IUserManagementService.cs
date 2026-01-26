using System.Collections.Generic;
using System.Threading.Tasks;
using VisitorLogSystem.DTOs;

namespace VisitorLogSystem.Interfaces
{
    
    /// User Management Service Interface
    /// Handles admin operations for managing users
   
    public interface IUserManagementService
    {
       
        /// Get all users in the system
        
        Task<List<UserDto>> GetAllUsersAsync();

       
        /// Get user by ID
        
        Task<UserDto?> GetUserByIdAsync(int id);

        
        /// Create new user account
        
        
        Task<UserDto> CreateUserAsync(string username, string password, string role);

        
        /// Update existing user
       
        
        Task<UserDto?> UpdateUserAsync(int id, string username, string? newPassword, string role);

       
        /// Delete user account
       
        
        Task<bool> DeleteUserAsync(int id, int currentUserId);

       
        /// Get user count by role
       
        Task<(int totalUsers, int admins, int staff)> GetUserStatisticsAsync();

        
        /// Check if username already exists
      
        Task<bool> UsernameExistsAsync(string username, int? excludeUserId = null);
    }
}