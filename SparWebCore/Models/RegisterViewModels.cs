using System;
using System.ComponentModel.DataAnnotations;

namespace SparWebCore.Models
{
    public class RegisterMainViewModel
    {
        public int? Mode { get; set; }
        public bool IsPopupMode { get; set; }
        public bool IsFromBlog { get; set; }
    }

    public class DateOfBirth
    {
        [Required]
        [Display(Name = "MM")]
        [Range(1, 12, ErrorMessage = "Please enter a month as a number from 1 to 12")]
        public int Month { get; set; }

        [Required]
        [Display(Name = "DD")]
        [Range(1, 31, ErrorMessage = "Please enter a day as a number from 1 to 31")]
        public int Day { get; set; }

        [Required]
        [Display(Name = "YYYY")]
        [RegularExpression(@"^\d{4}$", ErrorMessage = "Please enter a year as 4 digit number")]
        public int Year { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Date of birth")]
        public virtual DateOfBirth DateOfBirth { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        [Display(Name = "State / Province")]
        public string State { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        [Display(Name = "Email")]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Gym name")]
        public string GymName { get; set; }
    }

    public class RegisterFighterViewModel : RegisterViewModel
    {
        [Required]
        [Display(Name = "Gender")]
        public bool Sex { get; set; }

        [Required]
        [Display(Name = "Height")]
        public virtual double Height { get; set; }

        [Required]
        [Display(Name = "Weight class")]
        public virtual double Weight { get; set; }

        [Required]
        [Display(Name = "Stance")]
        public bool IsSouthpaw { get; set; }

        [Required]
        [Display(Name = "Amateur")]
        public virtual int NumberOfAmateurFights { get; set; }

        [Required]
        [Display(Name = "Professional")]
        public virtual int NumberOfProFights { get; set; }

        [Display(Name = "Your Rate")]
        public decimal? Rate { get; set; }

        [Display(Name = "Tell us more about yourself")]
        public string Comments { get; set; }

        public bool AddedByTrainer { get; set; }
        public string TrainerId { get; set; }
    }

    public class RegisterTrainerViewModel : RegisterViewModel
    {
        [Display(Name = "Phone Number")]
        [RegularExpression("^\\(?[2-9]\\d{2}(\\)\\s|[\\s\\.-])?[2-9]\\d{2}([\\s\\.-])?\\d{4}$", ErrorMessage = "Please enter correct phone number")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Website")]
        public string Website { get; set; }

        [Display(Name = "Rate (per hour)")]
        public decimal Rate { get; set; }

        [Display(Name = "Tell us more about yourself")]
        public string Notes { get; set; }
    }
}
