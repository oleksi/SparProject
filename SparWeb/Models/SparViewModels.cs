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

		[Display(Name = "Last Message")]
		[StringLength(1024)]
		public string SparNotes { get; set; }

		public string ThisFighterUrl
		{
			get
			{
				return String.Format("/fighters/{0}/{1}", Util.States[ThisFighter.State], ThisFighter.Name);
			}
		}

		public string OpponentFighterUrl
		{
			get
			{
				return String.Format("/fighters/{0}/{1}", Util.States[OpponentFighter.State], OpponentFighter.Name);
			}
		}
	}

	public class SparActivityViewModel : SparConfirmationViewModel
	{
		public SparActivityViewModel() { }

		public SparActivityViewModel(SparConfirmationViewModel sparConfirmationViewModel)
		{
			this.ThisFighter = sparConfirmationViewModel.ThisFighter;
			this.OpponentFighter = sparConfirmationViewModel.OpponentFighter;
			this.ProfilePictureSize = sparConfirmationViewModel.ProfilePictureSize;
		}

		public SparRequestStatus SparRequesStatus { get; set; }
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

		[Display(Name = "Spar Date")]
		public DateTime? SparDate { get; set; }

		[Display(Name = "Spar Time")]
		public SparTime SparTime { get; set; }

		public int? SparGymID { get; set; }

		[Display(Name = "Spar Location")]
		public Gym SparGym { get; set; }

		public int LastNegotiatorFighterId { get; set; }

		public SparRequestStatus SparRequesStatus { get; set; }

		public bool ListSparNotes { get; set; }

		public List<SparNoteViewModel> SparNotesList { get; set; }

		[StringLength(1024)]
		public string SparNotesNew { get; set; }

		public bool IsThisFighterLastNegotiator()
		{
			return (LastNegotiatorFighterId == ThisFighter.Id.Value);
		}
	}

	public class SelectFighterViewModel
	{
		public string SelectedFighterId { get; set; }
		public string OpponentFighterId { get; set; }

		public string OpponentFighterName { get; set; }

		public List<AccountFighterViewModel> FightersList { get; set; }
	}

	public class SparTime
	{
		[Display(Name = "HH")]
		public int? Hours { get; set; }

		[Display(Name = "MM")]
		public int? Minutes { get; set; }

		[Display(Name = "AM/PM")]
		public bool? IsAM { get; set; }

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
			if (Hours.HasValue && Minutes.HasValue && IsAM.HasValue)
				return String.Format("{0}:{1} {2}", Hours, Minutes.Value.ToString("00"), IsAM.Value ? "AM" : "PM");
			else
				return "N/A";
		}
	}
}