using System;
using System.ComponentModel.DataAnnotations;

namespace VisitorLogSystem.ViewModels
{
   
    /// ViewModel for Create/Edit/Display visitor forms
    /// Contains only fields needed by the UI with validation rules
   
    public class VisitorViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Full name is required")]
        [Display(Name = "Full Name")]
        [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
        public string FullName { get; set; } = string.Empty; 

        [Required(ErrorMessage = "Purpose is required")]
        [Display(Name = "Purpose of Visit")]
        [StringLength(500, ErrorMessage = "Purpose cannot exceed 500 characters")]
        public string Purpose { get; set; } = string.Empty;

        [Display(Name = "Contact Number")]
        [Phone(ErrorMessage = "Invalid phone number")]
        [StringLength(20)]
        public string? ContactNumber { get; set; } 

        [Display(Name = "Time In")]
        [DataType(DataType.DateTime)]
        public DateTime TimeIn { get; set; } = DateTime.Now; 

        [Display(Name = "Time Out")]
        [DataType(DataType.DateTime)]
        public DateTime? TimeOut { get; set; } 

        // Display-only properties
        [Display(Name = "Status")]
        public string Status => TimeOut.HasValue ? "Left" : "Inside";

        [Display(Name = "Duration")]
        public string Duration
        {
            get
            {
                if (!TimeOut.HasValue)
                    return "Still inside";

                var duration = TimeOut.Value - TimeIn;
                return $"{duration.Hours}h {duration.Minutes}m";
            }
        }
    }
}