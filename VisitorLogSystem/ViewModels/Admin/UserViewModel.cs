using System;
using System.ComponentModel.DataAnnotations;

namespace VisitorLogSystem.ViewModels.Admin
{
    
    /// ViewModel for displaying user information in admin panel
   
    public class UserViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Username")]
        public string Username { get; set; } = string.Empty;

        [Display(Name = "Role")]
        public string Role { get; set; } = string.Empty;

        [Display(Name = "Created At")]
        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; }

        
        /// Display-friendly role badge
       
        public string RoleBadgeClass => Role == "Admin" ? "bg-warning text-dark" : "bg-info";

     
        /// Format created date for display
     
        public string CreatedAtDisplay => CreatedAt.ToString("MMM dd, yyyy");
    }
}