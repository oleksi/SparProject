using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SparWeb.Models
{
	public enum AgeRange
	{
		AgeBelow12 = 1,
		Between12And16 = 2,
		Between16And20 = 3,
		Between20And24 = 4,
		Between24And30 = 5,
		Between30And36 = 6,
		Above36 = 7
	}

	public class HomeViewModel
	{
		public List<AccountViewModel> FightersList { get; set; }

		[Display(Name = "Age Range")]
		public AgeRange AgeRange { get; set; }

		[Display(Name = "Weight class")]
		public virtual double Weight { get; set; }

		[Display(Name = "Height")]
		public virtual double Height { get; set; }

		[Display(Name = "Gender")]
		public bool Sex { get; set; }
	}
}