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
	}
}
