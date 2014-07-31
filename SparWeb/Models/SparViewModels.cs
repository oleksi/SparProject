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
}