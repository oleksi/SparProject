using SparModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SparWeb.Models
{
	public class HomeViewModel
	{
		public List<AccountTrainerViewModel> FeaturedTrainersList { get; set; }
		public List<GymViewModel> FeaturedGymsList { get; set; }
		public List<AccountFighterViewModel> FeaturedFightersList { get; set; }
		public List<SiteActivityViewModel> RecentActivities { get; set; }
	}

	public class ActivityViewModel
	{
		public SparRequest SparRequest { get; set; }
		public Fighter Fighter { get; set; }
	}
}
