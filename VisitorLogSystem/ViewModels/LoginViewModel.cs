using System.ComponentModel.DataAnnotations;
using VisitorLogSystem.Models;

namespace VisitorLogSystem.ViewModels
{
    
    /// Used for the login form view
   
    /// DIFFERENCE FROM DTO:
    /// - ViewModel: Has validation attributes for UI
    /// - ViewModel: Has display names for labels
    /// - DTO: Plain data transfer, no UI stuff
    
    public class LoginViewModel
    {
       
        /// Username field
      
       
        [Required(ErrorMessage = "Username is required")]
        [Display(Name = "Username")]
        public string Username { get; set; } = string.Empty;

       
        /// Password field
        /// [DataType.Password] = Shows ••••• instead of text
        
        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

       
       
        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }

       
        
        public string? ErrorMessage { get; set; }
    }
}