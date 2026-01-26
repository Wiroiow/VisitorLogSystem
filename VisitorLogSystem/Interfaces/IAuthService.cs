using System.Security.Claims;
using System.Threading.Tasks;
using VisitorLogSystem.DTOs;

namespace VisitorLogSystem.Interfaces
{
    
    /// authentication logic
    /// RESPONSIBILITY: Handle login, logout, claims creation
   
    public interface IAuthService
    {
        
        /// Returns: UserDto if login successful, null if failed
      
        Task<UserDto?> ValidateLoginAsync(LoginDto loginDto);

        /// Create claims for authenticated user
        /// Returns: ClaimsPrincipal to store in cookie
        ClaimsPrincipal CreateClaimsPrincipal(UserDto user);

       
        bool VerifyPassword(string plainPassword, string passwordHash);
    }
}