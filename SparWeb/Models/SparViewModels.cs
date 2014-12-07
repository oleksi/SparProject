using SparModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SparWeb.Models
{
	public class SparConfirmationViewModel
	{
		public Fighter ThisFighter { get; set; }
		public Fighter OpponentFighter { get; set; }
		public int ProfilePictureSize { get; set; }
	}

	public class ConfirmSparDetailsViewModel : SparConfirmationViewModel
	{
		public ConfirmSparDetailsViewModel() { }
		public ConfirmSparDetailsViewModel(SparConfirmationViewModel sparConfirmationViewModel)
		{
			this.ThisFighter = sparConfirmationViewModel.ThisFighter;
			this.OpponentFighter = sparConfirmationViewModel.OpponentFighter;
			this.ProfilePictureSize = sparConfirmationViewModel.ProfilePictureSize;
		}

		[HiddenInput(DisplayValue = false)]
		public string SparRequestId { get; set; }

		[Required]
		[Display(Name = "Spar Date")]
		public DateTime? SparDate { get; set; }

		[Display(Name = "Spar Time")]
		public SparTime SparTime { get; set; }

		public Gym YourGym { get; set; }

		public Gym OpponentGym { get; set; }

		public int SparGymID { get; set; }

		[Display(Name = "Spar Location")]
		public Gym SparGym { get; set; }

		[Display(Name = "Notes")]
		[StringLength(1024)]
		public string SparNotes { get; set; }

		public SparRequestStatus SparRequesStatus { get; set; }
	}

	public class SparTime
	{
		[Display(Name = "HH")]
		public int Hours { get; set; }

		[Display(Name = "MM")]
		public int Minutes { get; set; }

		[Display(Name = "AM/PM")]
		public bool IsAM { get; set; }

		public SparTime()
		{
		}

		public SparTime(DateTime dateTime)
		{
			Hours = Convert.ToInt32(dateTime.ToString("hh"));
			Minutes = dateTime.Minute;
			IsAM = (dateTime.ToString("tt") == "AM");
		}

		public override string ToString()
		{
			return String.Format("{0}:{1} {2}", Hours, Minutes.ToString("00"), IsAM ? "AM" : "PM");
		}
	}
}