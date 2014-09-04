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
		public string ThisFighterID { get; set; }
		public string ThisFighterName { get; set; }
		public string ThisFighterGymName { get; set; }
		public Gym ThisFighterGym { get; set; }
		public string ThisProfilePictureFile { get; set; }
		public bool ThisProfilePictureUploaded { get; set; }
		public string OpponentFighterID { get; set; }
		public string OpponentFighterName { get; set; }
		public string OpponentFighterGymName { get; set; }
		public Gym OpponentFighterGym { get; set; }
		public string OpponentProfilePictureFile { get; set; }
		public bool OpponentProfilePictureUploaded { get; set; }
		public int ProfilePictureSize { get; set; }
	}

	public class ConfirmSparDetailsViewModel : SparConfirmationViewModel
	{
		public ConfirmSparDetailsViewModel() { }
		public ConfirmSparDetailsViewModel(SparConfirmationViewModel sparConfirmationViewModel)
		{
			this.ThisFighterID = sparConfirmationViewModel.ThisFighterID;
			this.ThisFighterName = sparConfirmationViewModel.ThisFighterName;
			this.ThisFighterGymName = sparConfirmationViewModel.ThisFighterGymName;
			this.ThisFighterGym = sparConfirmationViewModel.ThisFighterGym;
			this.ThisProfilePictureFile = sparConfirmationViewModel.ThisProfilePictureFile;
			this.ThisProfilePictureUploaded = sparConfirmationViewModel.ThisProfilePictureUploaded;
			this.OpponentFighterID = sparConfirmationViewModel.OpponentFighterID;
			this.OpponentFighterName = sparConfirmationViewModel.OpponentFighterName;
			this.OpponentFighterGymName = sparConfirmationViewModel.OpponentFighterGymName;
			this.OpponentFighterGym = sparConfirmationViewModel.OpponentFighterGym;
			this.OpponentProfilePictureFile = sparConfirmationViewModel.OpponentProfilePictureFile;
			this.OpponentProfilePictureUploaded = sparConfirmationViewModel.OpponentProfilePictureUploaded;
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

		public int? SparGymID { get; set; }

		[Display(Name = "Notes")]
		[StringLength(1024)]
		public string SparNotes { get; set; }
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
	}
}