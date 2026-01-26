using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VisitorLogSystem.Data;
using VisitorLogSystem.DTOs;
using VisitorLogSystem.Interfaces;
using VisitorLogSystem.Models;

namespace VisitorLogSystem.Services
{
    public class UserManagementService : IUserManagementService
    {
        private readonly IUserRepository _userRepository;
        private readonly ApplicationDbContext _context;

        public UserManagementService(IUserRepository userRepository, ApplicationDbContext context)
        {
            _userRepository = userRepository;
            _context = context;
        }

        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            var users = await _context.Users
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();

            return users.Select(MapToDto).ToList();
        }

        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);

            
            if (user == null)
                return null;

            return MapToDto(user);
        }

        public async Task<UserDto> CreateUserAsync(string username, string password, string role)
        {
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

            var user = new User
            {
                Username = username,
                PasswordHash = passwordHash,
                Role = role,
                CreatedAt = DateTime.Now
            };

            var createdUser = await _userRepository.CreateUserAsync(user);

            return MapToDto(createdUser);
        }

        public async Task<UserDto?> UpdateUserAsync(int id, string username, string? newPassword, string role)
        {
            var user = await _userRepository.GetUserByIdAsync(id);

            
            if (user == null)
            {
                return null;
            }

            user.Username = username;
            user.Role = role;

            if (!string.IsNullOrEmpty(newPassword))
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            }

            var updatedUser = await _userRepository.UpdateAsync(user);

            
            if (updatedUser == null)
            {
                return null;
            }

            return MapToDto(updatedUser);
        }

        public async Task<bool> DeleteUserAsync(int id, int currentUserId)
        {
            //Prevent deleting yourself
            if (id == currentUserId)
            {
                return false;
            }

            return await _userRepository.DeleteAsync(id);
        }

        public async Task<(int totalUsers, int admins, int staff)> GetUserStatisticsAsync()
        {
            var totalUsers = await _context.Users.CountAsync();
            var admins = await _context.Users.CountAsync(u => u.Role == "Admin");
            var staff = await _context.Users.CountAsync(u => u.Role == "Staff");

            return (totalUsers, admins, staff);
        }

        public async Task<bool> UsernameExistsAsync(string username, int? excludeUserId = null)
        {
            var query = _context.Users.Where(u => u.Username == username);

            if (excludeUserId.HasValue)
            {
                query = query.Where(u => u.Id != excludeUserId.Value);
            }

            return await query.AnyAsync();
        }

        #region Helper Methods

        private UserDto MapToDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Role = user.Role,
                CreatedAt = user.CreatedAt
            };
        }

        #endregion
    }
}