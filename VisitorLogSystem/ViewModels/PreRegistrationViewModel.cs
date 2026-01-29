using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VisitorLogSystem.DTOs;

namespace VisitorLogSystem.ViewModels
{
    public class PreRegistrationViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Full Name is required")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Purpose is required")]
        [Display(Name = "Purpose of Visit")]
        public string? Purpose { get; set; } 

        [Required(ErrorMessage = "Expected Visit Date is required")]
        [Display(Name = "Expected Visit Date")]
        [DataType(DataType.DateTime)]
        public DateTime ExpectedVisitDate { get; set; }

        [Required]
        [Display(Name = "Host")]
        public int HostUserId { get; set; }

        public string? HostUserName { get; set; } 
        public bool IsCheckedIn { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CheckedInByUserName { get; set; }
        public DateTime CheckedInAt { get; set; } 
        public int? RoomVisitId { get; set; }

        
        public List<UserDto>? AvailableHosts{ get; set; } 
    }

    public class PreRegistrationListViewModel
    {
        public List<PreRegistrationViewModel> PreRegistrations { get; set; } = new List<PreRegistrationViewModel>();

        public string? SearchTerm { get; set; }
        public DateTime? FilterDate { get; set; }
        public bool ShowOnlyPending { get; set; }
    }
}