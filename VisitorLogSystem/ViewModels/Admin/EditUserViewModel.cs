using System;
using System.ComponentModel.DataAnnotations;

namespace VisitorLogSystem.ViewModels.Admin
{
  
    /// Password is optional - only change if provided
   
    public class EditUserViewModel
    {
        public int Id { get; set; }

        
        
        
        [Required(ErrorMessage = "Username is required")]
        [Display(Name = "Username")]
        [StringLength(50, MinimumLength = 3)]
        public string Username { get; set; } = string.Empty;

       
        /// New password (optional)
      
        [Display(Name = "New Password")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
        [DataType(DataType.Password)]
        public string? NewPassword { get; set; }

       
        [Display(Name = "Confirm New Password")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        public string? ConfirmPassword { get; set; }

      
        [Required(ErrorMessage = "Role is required")]
        [Display(Name = "Role")]
        public string Role { get; set; } = string.Empty;

        
        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; }
    }
}