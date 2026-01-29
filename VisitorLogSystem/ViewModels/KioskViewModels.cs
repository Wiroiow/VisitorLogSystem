using System;
using System.ComponentModel.DataAnnotations;

namespace VisitorLogSystem.ViewModels
{
   
    /// ViewModel for pre-registration lookup screen
   
    public class KioskPreRegLookupViewModel
    {
        [Required(ErrorMessage = "Please enter your full name")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string FullName { get; set; } = string.Empty;
    }

   
    /// ViewModel for visitor details + room selection (combined screen)
    
    public class KioskCheckInViewModel
    {
        
        public int? PreRegistrationId { get; set; }
        public bool IsPreRegistered { get; set; }
        public string? PreRegPurpose { get; set; }
        public string? PreRegRoomName { get; set; }
        public int? HostUserId { get; set; }

        
        [Required(ErrorMessage = "Full name is required")]
        [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Purpose of visit is required")]
        [StringLength(500, ErrorMessage = "Purpose cannot exceed 500 characters")]
        public string Purpose { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Please enter a valid phone number")]
        [StringLength(20, ErrorMessage = "Contact number cannot exceed 20 characters")]
        public string? ContactNumber { get; set; }

        
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Please select a room")]
        [StringLength(100)]
        public string RoomName { get; set; } = string.Empty;
    }

    /// ViewModel for success confirmation screen
   
    public class KioskSuccessViewModel
    {
        public string FullName { get; set; } = string.Empty;
        public string RoomName { get; set; } = string.Empty;
        public string Purpose { get; set; } = string.Empty;
        public DateTime CheckInTime { get; set; }
        public bool WasPreRegistered { get; set; }
    }
}