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
		[Display(Name = "Date of birth")]
		public virtual DateOfBirth DateOfBirth { get; set; }

		[Required]
		public string City { get; set; }

		[Required]
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

		[Display(Name = "Notes")]
		public string Notes { get; set; }
	}

	public class AccountViewModel
	{
		public string ID { get; set; }

		public string Name { get; set; }

		[Required]
		[Display(Name = "City:")]
		public string City { get; set; }

		[Required]
		[Display(Name = "State:")]
		public string State { get; set; }

		[Display(Name = "Age:")]
		public int Age { get; set; }

		public string ProfilePictureFile { get; set; }

		public bool ProfilePictureUploaded { get; set; }

		[Display(Name = "Gym name:")]
		public string GymName { get; set; }
		public Gym Gym { get; set; }

		public bool IsTrainerView { get; set; }
		public bool IsFighterSelectView { get; set; }
		public bool IsWidgetView { get; set; }
	}

	public class AccountFighterViewModel : AccountViewModel
	{
		[Required]
		[Display(Name = "Height:")]
		public double Height { get; set; }

		[Required]
		[Display(Name = "Weight class:")]
		public double Weight { get; set; }

		[Required]
		[Display(Name = "Stance:")]
		public bool IsSouthpaw { get; set; }

		[Required]
		[Display(Name = "Amateur")]
		public int NumberOfAmateurFights { get; set; }

		[Required]
		[Display(Name = "Professional")]
		public int NumberOfProFights { get; set; }

		public string HimOrHer { get; set; }

		public IList<ConfirmSparDetailsViewModel> SparRequests { get; set; }

		public AccountFighterViewModel()
		{
			SparRequests = new List<ConfirmSparDetailsViewModel>();
		}

		public string FighterUrl
		{
			get 
			{ 
				return String.Format("/fighters/{0}/{1}", Util.States[State], Name); 
			}
		}
	}

	public class AccountTrainerViewModel : AccountViewModel
	{
		[Display(Name = "Phone Number:")]
		[RegularExpression("^\\(?[2-9]\\d{2}(\\)\\s|[\\s\\.-])?[2-9]\\d{2}([\\s\\.-])?\\d{4}$", ErrorMessage = "Please enter correct phone number")]
		public string PhoneNumber { get; set; }

		[Display(Name = "Website:")]
		public string Website { get; set; }

		[Display(Name = "Rate (per hour):")]
		public decimal Rate { get; set; }

		[Display(Name = "Notes:")]
		public string Notes { get; set; }

		public IList<ConfirmSparDetailsViewModel> SparRequests { get; set; }

		public List<AccountFighterViewModel> FightersList { get; set; }

		public AccountTrainerViewModel()
		{
			FightersList = new List<AccountFighterViewModel>();
		}

		public string GetHtmlFormattedNotes()
		{
			return (Notes != null)? Notes.Replace("\n", "<br />") : "";
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
