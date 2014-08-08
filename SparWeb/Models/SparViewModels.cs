using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SparWeb.Models
{
	public class SparConfirmationViewModel
	{
		public string ThisFighterID { get; set; }
		public string ThisFighterName { get; set; }
		public string ThisFighterGymName { get; set; }
		public string ThisProfilePictureFile { get; set; }
		public bool ThisProfilePictureUploaded { get; set; }
		public string OpponentFighterID { get; set; }
		public string OpponentFighterName { get; set; }
		public string OpponentFighterGymName { get; set; }
		public string OpponentProfilePictureFile { get; set; }
		public bool OpponentProfilePictureUploaded { get; set; }
	}

	public class ConfirmSparDetailsViewModel : SparConfirmationViewModel
	{
		public ConfirmSparDetailsViewModel() { }
		public ConfirmSparDetailsViewModel(SparConfirmationViewModel sparConfirmationViewModel)
		{
			this.ThisFighterID = sparConfirmationViewModel.ThisFighterID;
			this.ThisFighterName = sparConfirmationViewModel.ThisFighterName;
			this.ThisFighterGymName = sparConfirmationViewModel.ThisFighterGymName;
			this.ThisProfilePictureFile = sparConfirmationViewModel.ThisProfilePictureFile;
			this.ThisProfilePictureUploaded = sparConfirmationViewModel.ThisProfilePictureUploaded;
			this.OpponentFighterID = sparConfirmationViewModel.OpponentFighterID;
			this.OpponentFighterName = sparConfirmationViewModel.OpponentFighterName;
			this.OpponentFighterGymName = sparConfirmationViewModel.OpponentFighterGymName;
			this.OpponentProfilePictureFile = sparConfirmationViewModel.OpponentProfilePictureFile;
			this.OpponentProfilePictureUploaded = sparConfirmationViewModel.OpponentProfilePictureUploaded;
		}

		public string SparRequestId { get; set; }

	}
}