﻿using SparModel;
using SparWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SparWeb
{
	public partial class Util
	{
		public static Dictionary<double, string> HeightToCentimetersMap = new Dictionary<double, string>() 
		{
			{1, "less than 4'8\""},
			{142, "4'8\""},
			{144.5, "4'9\""},
			{147, "4'10\""},
			{150, "4'11\""},
			{152.5, "5'"},
			{155, "5'1\""},
			{157.5, "5'2\""},
			{160, "5'3\""},
			{162.5, "5'4\""},
			{165, "5'5\""},
			{167.5, "5'6\""},
			{170, "5'7\""},
			{172.5, "5'8\""},
			{175, "5'9\""},
			{177.5, "5'10\""},
			{180, "5'11\""},
			{183, "6'"},
			{185.5, "6'1\""},
			{188, "6'2\""},
			{190.5, "6'3\""},
			{1000, "greater than 6'3\""}
		};

		public static Dictionary<double, string> WeightClassMap = new Dictionary<double, string>()
		{
			{106, "below 106 lbs: Light Flyweight"},
			{112, "112 lbs: Flyweight"},
			{119, "119 lbs: Bantamweight"},
			{125, "125 lbs: Featherweight"},
			{132, "132 lbs: Lightweight"},
			{141, "141 lbs: Light Welterweight"},
			{152, "152 lbs: Welterweight"},
			{165, "165 lbs: Middleweight"},
			{178, "178 lbs: Light Heavyweight"},
			{201, "201 lbs: Heavyweight"},
			{1000, "over 201 lbs: Super Heavyweight"}
		};

		public static AccountViewModel GetAccountViewModelForFighter(Fighter fighter, int thumbnailSize)
		{
			string gymName = (fighter.Gym != null) ? fighter.Gym.Name : "Unknown Gym";

			AccountViewModel model = null;
			if (fighter != null)
				model = new AccountViewModel() { 
					ID = fighter.SparIdentityUser.Id,
					Name = fighter.Name, 
					GymName = gymName,
					Gym = fighter.Gym, 
					Age = fighter.getFighterAge(), 
					Height = Util.HeightToCentimetersMap[fighter.Height], 
					Weight = fighter.Weight, 
					NumberOfFights = fighter.NumberOfFights, 
					ProfilePictureUploaded = fighter.ProfilePictureUploaded,
					ProfilePictureFile = String.Format("{0}{1}", (fighter.ProfilePictureUploaded == true) ? System.Configuration.ConfigurationManager.AppSettings["ProfilePicsUrl"] : VirtualPathUtility.ToAbsolute("~/Content/Images/"), fighter.getProfileThumbnailFileName(thumbnailSize)),
					HimOrHer = fighter.GetHimOrHer(true)
				};

			return model;
		}
	}
}