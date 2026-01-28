using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisitorLogSystem.DTOs;

namespace VisitorLogSystem.Interfaces
{
    public interface IUserManagementService
    {
        // Synchronous version (for dropdowns, etc.)
        IEnumerable<UserDto> GetUsers();

        // Async version (for async operations)
        Task<List<UserDto>> GetAllUsersAsync();

        Task<UserDto?> GetUserByIdAsync(int id);
        Task<UserDto> CreateUserAsync(string username, string password, string role);
        Task<UserDto?> UpdateUserAsync(int id, string username, string? newPassword, string role);
        Task<bool> DeleteUserAsync(int id, int currentUserId);
        Task<(int totalUsers, int admins, int staff)> GetUserStatisticsAsync();
        Task<bool> UsernameExistsAsync(string username, int? excludeUserId = null);
    }
}