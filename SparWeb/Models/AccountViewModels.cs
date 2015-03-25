using SparModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SparWeb.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }
    }

	public class ManagePasswordViewModel
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
		public string Name { get; set; }

		[Required]
		[Display(Name = "Gender")]
		public bool Sex { get; set; }

		[Required]
		[Display(Name = "Date of birth")]
		public virtual DateOfBirth DateOfBirth { get; set; }

		[Required]
		[Display(Name = "Height")]
		public virtual double Height { get; set; }

		[Required]
		[Display(Name = "Weight class")]
		public virtual double Weight { get; set; }

		[Required]
		[Display(Name = "Number of fights (amateur)")]
		public virtual int? NumberOfFights { get; set; }

		[Display(Name = "Your Gym")]
		public virtual int? GymId { get; set; }

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

		public string NewGymName { get; set; }
    }

	public class AccountViewModel
	{
		public string ID { get; set; }

		public string Name { get; set; }

		public string GymName { get; set; }
		public Gym Gym { get; set; }

		[Display(Name = "Age")]
		public int Age { get; set; }

		[Display(Name = "Height")]
		public string Height { get; set; }

		[Display(Name = "Weight class")]
		public double Weight { get; set; }

		[Display(Name = "Number of fights")]
		public int NumberOfFights { get; set; }

		public string ProfilePictureFile { get; set; }

		public bool ProfilePictureUploaded { get; set; }

		public string HimOrHer { get; set; }

		public IList<ConfirmSparDetailsViewModel> SparRequests { get; set; }

		public AccountViewModel()
		{
			SparRequests = new List<ConfirmSparDetailsViewModel>();
		}
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

	public class VerifyCodeViewModel
	{
		[Required]
		public string Provider { get; set; }

		[Required]
		[Display(Name = "Code")]
		public string Code { get; set; }
		public string ReturnUrl { get; set; }

		[Display(Name = "Remember this browser?")]
		public bool RememberBrowser { get; set; }
	}

	public class ForgotPasswordViewModel
	{
		[Required]
		[EmailAddress]
		[Display(Name = "Email")]
		public string Email { get; set; }
	}

	public class ResetPasswordViewModel
	{
		[Required]
		[EmailAddress]
		[Display(Name = "Email")]
		public string Email { get; set; }

		[Required]
		[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
		[DataType(DataType.Password)]
		[Display(Name = "Password")]
		public string Password { get; set; }

		[DataType(DataType.Password)]
		[Display(Name = "Confirm password")]
		[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
		public string ConfirmPassword { get; set; }

		public string Code { get; set; }
	}

	public class SendCodeViewModel
	{
		public string SelectedProvider { get; set; }
		public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
		public string ReturnUrl { get; set; }
	}
}
