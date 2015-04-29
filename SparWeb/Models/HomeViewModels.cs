﻿using System;
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
		Between20And25 = 4,
		Between25And30 = 5,
		Between30And35 = 6,
		Between35And40 = 7,
		Between40And45 = 8,
		Between45And50 = 9,
		Between50And55 = 10,
		Between55And60 = 11,
		Above60 = 12
	}

	public enum NumberOfFights
	{
		NoFights = 1,
		Between1And5Fights = 2,
		Betwee5nAnd10Fights = 3,
		Between10And15Fights = 4,
		Between15And20Fights = 5,
		Between20And30Fights = 6,
		Between30And40Fights = 7,
		MoreThan40 = 8
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

		[Display(Name = "Stance")]
		public bool? IsSouthpaw { get; set; }

		[Display(Name = "Number of fights")]
		public virtual NumberOfFights NumberOfFights { get; set; }

		[Display(Name = "Gender")]
		public bool? Sex { get; set; }
	}
}