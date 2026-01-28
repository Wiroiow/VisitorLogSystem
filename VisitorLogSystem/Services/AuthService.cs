using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics.Arm;
using System.Security.Claims;
using System.Threading.Tasks;
using VisitorLogSystem.DTOs;
using VisitorLogSystem.Interfaces;
using VisitorLogSystem.Models;

namespace VisitorLogSystem.Services
{
    
    /// Authentication Service - Handles all authentication logic
  
   
    
    /// NO direct database access (uses Repository)
    
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;




        public AuthService(IUserRepository userRepository, IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
        }


        
        /// Validate login credentials
        
        
        
        public async Task<UserDto?> ValidateLoginAsync(LoginDto loginDto)
        {
            
            

            // Find user by username
            // Ask repository to search database
            var user = await _userRepository.GetUserByUsernameAsync(loginDto.Username);

           
            if (user == null)
            {
                
                return null;
            }

           
            

          

            bool isPasswordValid = VerifyPassword(loginDto.Password, user.PasswordHash);

            if (!isPasswordValid)
            {
              
            }

            
            

            
            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Role = user.Role,
                CreatedAt = user.CreatedAt
            };
        }

       
        public bool VerifyPassword(string plainPassword, string passwordHash)
        {
            try
            {
                
                bool result = BCrypt.Net.BCrypt.Verify(plainPassword, passwordHash);

               

                return result;
            }
            catch (Exception ex)
            {
                
                Console.WriteLine($"❌ EXCEPTION during password verification:");
                Console.WriteLine($"   Error: {ex.Message}");
                Console.WriteLine($"   💡 This might mean the hash format is invalid!");
                return false;
            }
        }

        
        public ClaimsPrincipal CreateClaimsPrincipal(UserDto user)
        {
            // Create list of claims (ID card info)
            var claims = new List<Claim>
            {
                
                
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                
               
                
                new Claim(ClaimTypes.Name, user.Username),
                
               
               
                new Claim(ClaimTypes.Role, user.Role)
            };

           
            // "Cookie" = authentication type (we're using cookies)
            var identity = new ClaimsIdentity(claims, "Cookie");

           
            // This is what gets stored in the cookie
            var principal = new ClaimsPrincipal(identity);
            return principal;
        }
        public int GetCurrentUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
            return 0;
        }
    }
}
