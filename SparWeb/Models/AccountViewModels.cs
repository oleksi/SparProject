using SparModel;
using System;
using System.ComponentModel.DataAnnotations;

namespace SparWeb.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }
    }

    public class ManageUserViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
		[DataType(DataType.EmailAddress)]
		[EmailAddress]
		[Display(Name = "Email")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
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

		[Required]
		public string Name { get; set; }

		[Required]
		[Display(Name="Gender")]
		public bool Sex { get; set; }

		[Required]
		[Display(Name="Date of birth")]
		public virtual DateOfBirth DateOfBirth { get; set; }

		[Required]
		[Display(Name="Height")]
		public virtual double Height { get; set; }
    }

	public class DateOfBirth
	{
		[Required]
		[Display(Name = "MM")]
		[Range(1, 31, ErrorMessage = "Please enter a month as a number from 1 to 12")]
		public int Month { get; set; }

		[Required]
		[Display(Name = "DD")]
		[Range(1, 31, ErrorMessage = "Please enter a day as a number from 1 to 31")]
		public int Day { get; set; }

		[Required]
		[Display(Name = "YYYY")]
		[RegularExpression(@"^\d{4}$", ErrorMessage = "Pleas enter a year as 4 digit number")]
		public int Year { get; set; }
	}
}
