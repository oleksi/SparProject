using SparData;
using SparModel;
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

		public static Dictionary<double, string> WeightClassShortMap = new Dictionary<double, string>()
		{
			{106, "below 106 lbs"},
			{112, "112 lbs"},
			{119, "119 lbs"},
			{125, "125 lbs"},
			{132, "132 lbs"},
			{141, "141 lbs"},
			{152, "152 lbs"},
			{165, "165 lbs"},
			{178, "178 lbs"},
			{201, "201 lbs"},
			{1000, "over 201 lbs"}
		};

		public static Dictionary<int, string> AgeRangeMap = new Dictionary<int, string>()
		{
			{1, "Below 12"},
			{2, "Between 12 and 16"},
			{3, "Between 16 and 20"},
			{4, "Between 20 and 25"},
			{5, "Between 25 and 30"},
			{6, "Between 30 and 35"},
			{7, "Between 35 and 40"},
			{8, "Between 40 and 45"},
			{9, "Between 45 and 50"},
			{10, "Between 50 and 55"},
			{11, "Between 55 and 60"},
			{12, "Above 60"}
		};

		public static Dictionary<int, string> NumberOfFights = new Dictionary<int, string>()
		{
			{1, "0 fights"},
			{2, "between 1 and 5 fights"},
			{3, "between 5 and 10 fights"},
			{4, "between 10 and 15 fights"},
			{5, "between 15 and 20 fights"},
			{6, "between 20 and 30 fights"},
			{7, "between 30 and 40 fights"},
			{8, "more than 40 fights"}
		};

		public static Dictionary<string, string> States = new Dictionary<string, string>()
		{
			{"AL", "Alabama"},
			{"AK", "Alaska"},
			{"AZ", "Arizona"},
			{"AR", "Arkansas"},
			{"CA", "California"},
			{"CO", "Colorado"},
			{"CT", "Connecticut"},
			{"DC", "District of Columbia"},
			{"DE", "Delaware"},
			{"FL", "Florida"},
			{"GA", "Georgia"},
			{"HI", "Hawaii"},
			{"ID", "Idaho"},
			{"IL", "Illinois"},
			{"IN", "Indiana"},
			{"IA", "Iowa"},
			{"KS", "Kansas"},
			{"KY", "Kentucky"},
			{"LA", "Louisiana"},
			{"ME", "Maine"},
			{"MD", "Maryland"},
			{"MA", "Massachusetts"},
			{"MI", "Michigan"},
			{"MN", "Minnesota"},
			{"MS", "Mississippi"},
			{"MO", "Missouri"},
			{"MT", "Montana"},
			{"NE", "Nebraska"},
			{"NV", "Nevada"},
			{"NH", "New Hampshire"},
			{"NJ", "New Jersey"},
			{"NM", "New Mexico"},
			{"NY", "New York"},
			{"NC", "North Carolina"},
			{"ND", "North Dakota"},
			{"OH", "Ohio"},
			{"OK", "Oklahoma"},
			{"OR", "Oregon"},
			{"PA", "Pennsylvania"},
			{"RI", "Rhode Island"},
			{"SC", "South Carolina"},
			{"SD", "South Dakota"},
			{"TN", "Tennessee"},
			{"TX", "Texas"},
			{"UT", "Utah"},
			{"VT", "Vermont"},
			{"VA", "Virginia"},
			{"WA", "Washington"},
			{"WV", "West Virginia"},
			{"WI", "Wisconsin"},
			{"WY", "Wyoming"}
		};

		public static AccountFighterViewModel GetAccountViewModelForFighter(Fighter fighter, int thumbnailSize)
		{
			string gymName = (fighter.Gym != null) ? fighter.Gym.Name : "Unknown Gym";

			AccountFighterViewModel model = null;
			if (fighter != null)
				model = new AccountFighterViewModel() { 
					ID = fighter.SparIdentityUser.Id,
					Name = fighter.Name, 
					GymName = gymName,
					Gym = fighter.Gym, 
					City = fighter.City,
					State = fighter.State,
					Age = fighter.GetMemberAge(), 
					Height = fighter.Height, 
					Weight = fighter.Weight, 
					IsSouthpaw = fighter.IsSouthpaw,
					NumberOfAmateurFights = fighter.NumberOfAmateurFights, 
					NumberOfProFights = fighter.NumberOfProFights,
					ProfilePictureUploaded = fighter.ProfilePictureUploaded,
					ProfilePictureFile = GetProfilePictureFile(fighter, thumbnailSize),
					HimOrHer = fighter.GetHimOrHer(true)
				};

			return model;
		}

		public static AccountTrainerViewModel GetAccountViewModelForTrainer(Trainer trainer, int thumbnailSize)
		{
			string gymName = (trainer.Gym != null) ? trainer.Gym.Name : "Unknown Gym";

			AccountTrainerViewModel model = null;
			if (trainer != null)
				model = new AccountTrainerViewModel()
				{
					ID = trainer.SparIdentityUser.Id,
					Name = trainer.Name,
					GymName = gymName,
					Gym = trainer.Gym,
					City = trainer.City,
					State = trainer.State,
					Age = trainer.GetMemberAge(),
					ProfilePictureUploaded = trainer.ProfilePictureUploaded,
					ProfilePictureFile = GetProfilePictureFile(trainer, thumbnailSize)
				};

			return model;
		}

		public static string GetProfilePictureFile(Member member, int thumbnailSize)
		{
			return member.GetProfilePictureFile(thumbnailSize, System.Configuration.ConfigurationManager.AppSettings["ProfilePicsUrl"], VirtualPathUtility.ToAbsolute("~/Content/Images/"));
		}

		public static ConfirmSparDetailsViewModel GetConfirmSparDetailsViewModel(SparRequest sparRequest, int profilePictureSize, string currUserId)
		{
			//figuring out who is who
			Fighter thisFighter = null;
			Fighter opponentFighter = null;
			if (sparRequest.RequestorFighter.SparIdentityUser.Id == currUserId)
			{
				thisFighter = sparRequest.RequestorFighter;
				opponentFighter = sparRequest.OpponentFighter;
			}
			else
			{
				thisFighter = sparRequest.OpponentFighter;
				opponentFighter = sparRequest.RequestorFighter; ;
			}

			ConfirmSparDetailsViewModel confirmSparDetailsViewModel = new ConfirmSparDetailsViewModel(GetSparConfirmationViewModel(thisFighter, opponentFighter, profilePictureSize));

			confirmSparDetailsViewModel.SparRequestId = sparRequest.Id;

			if (sparRequest.SparDateTime.HasValue == true)
			{
				confirmSparDetailsViewModel.SparDate = sparRequest.SparDateTime.Value.Date;
				confirmSparDetailsViewModel.SparTime = new SparTime(sparRequest.SparDateTime.Value);
			}

			if (sparRequest.SparGym != null)
			{
				confirmSparDetailsViewModel.SparGymID = sparRequest.SparGym.Id.Value;
				confirmSparDetailsViewModel.SparGym = sparRequest.SparGym;
			}

			//if Requested status, setting SparGymId to this fighters gym so current fighte's gym is selected by default
			if (sparRequest.Status == SparRequestStatus.Requested && thisFighter.Gym != null)
				confirmSparDetailsViewModel.SparGymID = thisFighter.Gym.Id.Value;

			confirmSparDetailsViewModel.SparNotes = sparRequest.SparNotes;
			confirmSparDetailsViewModel.SparRequesStatus = sparRequest.Status;

			confirmSparDetailsViewModel.LastNegotiatorFighterId = sparRequest.LastNegotiatorFighterId;

			return confirmSparDetailsViewModel;
		}

		public static SparConfirmationViewModel GetSparConfirmationViewModel(Fighter thisFighter, Fighter opponentFighter, int profilePictureSize)
		{
			SparConfirmationViewModel sparConfirmationViewModel = new SparConfirmationViewModel()
			{
				ThisFighter = thisFighter,
				OpponentFighter = opponentFighter,
				ProfilePictureSize = profilePictureSize
			};
			return sparConfirmationViewModel;
		}

		public static IList<ConfirmSparDetailsViewModel> GetSparRequestDetailsForFighter(int fighterId, string currUserId)
		{
			SparRepository sparRepo = new SparRepository();
			return sparRepo.GetSparRequestsForFighter(fighterId).Select(sr => Util.GetConfirmSparDetailsViewModel(sr, 150, currUserId)).ToList();
		}

		public static void PopualateRegistrationDropdowns(dynamic viewBag)
		{
			viewBag.States = Util.States;
			viewBag.HeightToCentimetersMap = Util.HeightToCentimetersMap;
			viewBag.WeightClassMap = Util.WeightClassMap;
		}
	}
}