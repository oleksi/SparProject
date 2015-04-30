using SparData;
using SparModel;
using SparWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Configuration;

namespace SparWeb.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			FighterRepository fighterRepo = new FighterRepository();
			SparRepository sparRepo = new SparRepository();

			IList<Fighter> fightersList = fighterRepo.GetAllFighters();

			HomeViewModel model = new HomeViewModel();
			model.FightersList = getFightersListViewModel(fightersList);

			populateFilterDropdowns();

			return View(model);
		}

		[HttpPost]
		public ActionResult Index(HomeViewModel model)
		{
			FighterRepository fighterRepo = new FighterRepository();
			IList<Fighter> fightersList = null;

			//filter by age
			if (model.AgeRange != 0)
			{
				if (model.AgeRange == AgeRange.AgeBelow12)
					fightersList = fighterRepo.GetAllFighters().Where(ff => getAge(ff.DateOfBirth) < 12).ToList();
				else if (model.AgeRange == AgeRange.Between12And16)
					fightersList = fighterRepo.GetAllFighters().Where(ff => getAge(ff.DateOfBirth) >= 12 && getAge(ff.DateOfBirth) < 16).ToList();
				else if (model.AgeRange == AgeRange.Between16And20)
					fightersList = fighterRepo.GetAllFighters().Where(ff => getAge(ff.DateOfBirth) >= 16 && getAge(ff.DateOfBirth) < 20).ToList();
				else if (model.AgeRange == AgeRange.Between20And25)
					fightersList = fighterRepo.GetAllFighters().Where(ff => getAge(ff.DateOfBirth) >= 20 && getAge(ff.DateOfBirth) < 25).ToList();
				else if (model.AgeRange == AgeRange.Between25And30)
					fightersList = fighterRepo.GetAllFighters().Where(ff => getAge(ff.DateOfBirth) >= 25 && getAge(ff.DateOfBirth) < 30).ToList();
				else if (model.AgeRange == AgeRange.Between30And35)
					fightersList = fighterRepo.GetAllFighters().Where(ff => getAge(ff.DateOfBirth) >= 30 && getAge(ff.DateOfBirth) < 35).ToList();
				else if (model.AgeRange == AgeRange.Between35And40)
					fightersList = fighterRepo.GetAllFighters().Where(ff => getAge(ff.DateOfBirth) >= 35 && getAge(ff.DateOfBirth) < 40).ToList();
				else if (model.AgeRange == AgeRange.Between40And45)
					fightersList = fighterRepo.GetAllFighters().Where(ff => getAge(ff.DateOfBirth) >= 40 && getAge(ff.DateOfBirth) < 45).ToList();
				else if (model.AgeRange == AgeRange.Between45And50)
					fightersList = fighterRepo.GetAllFighters().Where(ff => getAge(ff.DateOfBirth) >= 45 && getAge(ff.DateOfBirth) < 50).ToList();
				else if (model.AgeRange == AgeRange.Between50And55)
					fightersList = fighterRepo.GetAllFighters().Where(ff => getAge(ff.DateOfBirth) >= 50 && getAge(ff.DateOfBirth) < 55).ToList();
				else if (model.AgeRange == AgeRange.Between55And60)
					fightersList = fighterRepo.GetAllFighters().Where(ff => getAge(ff.DateOfBirth) >= 55 && getAge(ff.DateOfBirth) < 60).ToList();
				else if (model.AgeRange == AgeRange.Above60)
					fightersList = fighterRepo.GetAllFighters().Where(ff => getAge(ff.DateOfBirth) >= 36).ToList();
			}

			//filter by weight
			if (model.Weight != 0)
			{
				if (fightersList == null)
					fightersList = fighterRepo.GetAllFighters().Where(ff => ff.Weight == model.Weight).ToList();
				else
					fightersList = fightersList.Where(ff => ff.Weight == model.Weight).ToList();
			}

			//filter by height
			if (model.Height != 0)
			{
				if (fightersList == null)
					fightersList = fighterRepo.GetAllFighters().Where(ff => ff.Height == model.Height).ToList();
				else
					fightersList = fightersList.Where(ff => ff.Height == model.Height).ToList();
			}

			//filter by stance
			if (model.IsSouthpaw != null)
			{
				if (fightersList == null)
					fightersList = fighterRepo.GetAllFighters().Where(ff => ff.IsSouthpaw == model.IsSouthpaw.Value).ToList();
				else
					fightersList = fightersList.Where(ff => ff.IsSouthpaw == model.IsSouthpaw.Value).ToList();
			}

			if (model.NumberOfFights != 0)
			{
				if (model.NumberOfFights == NumberOfFights.NoFights)
					fightersList = (fightersList == null) ? fighterRepo.GetAllFighters().Where(ff => ff.NumberOfFights == 0).ToList() : fightersList.Where(ff => ff.NumberOfFights == 0).ToList();
				else if (model.NumberOfFights == NumberOfFights.Between1And5Fights)
					fightersList = (fightersList == null) ? fighterRepo.GetAllFighters().Where(ff => ff.NumberOfFights > 0 && ff.NumberOfFights < 5).ToList() : fightersList.Where(ff => ff.NumberOfFights > 0 && ff.NumberOfFights < 5).ToList();
				else if (model.NumberOfFights == NumberOfFights.Betwee5nAnd10Fights)
					fightersList = (fightersList == null) ? fighterRepo.GetAllFighters().Where(ff => ff.NumberOfFights >= 5 && ff.NumberOfFights < 10).ToList() : fightersList.Where(ff => ff.NumberOfFights >= 5 && ff.NumberOfFights < 10).ToList();
				else if (model.NumberOfFights == NumberOfFights.Between10And15Fights)
					fightersList = (fightersList == null) ? fighterRepo.GetAllFighters().Where(ff => ff.NumberOfFights >= 10 && ff.NumberOfFights < 15).ToList() : fightersList.Where(ff => ff.NumberOfFights >= 10 && ff.NumberOfFights < 15).ToList();
				else if (model.NumberOfFights == NumberOfFights.Between15And20Fights)
					fightersList = (fightersList == null) ? fighterRepo.GetAllFighters().Where(ff => ff.NumberOfFights >= 15 && ff.NumberOfFights < 20).ToList() : fightersList.Where(ff => ff.NumberOfFights >= 15 && ff.NumberOfFights < 20).ToList();
				else if (model.NumberOfFights == NumberOfFights.Between20And30Fights)
					fightersList = (fightersList == null) ? fighterRepo.GetAllFighters().Where(ff => ff.NumberOfFights >= 20 && ff.NumberOfFights < 30).ToList() : fightersList.Where(ff => ff.NumberOfFights >= 20 && ff.NumberOfFights < 30).ToList();
				else if (model.NumberOfFights == NumberOfFights.Between30And40Fights)
					fightersList = (fightersList == null) ? fighterRepo.GetAllFighters().Where(ff => ff.NumberOfFights >= 30 && ff.NumberOfFights < 40).ToList() : fightersList.Where(ff => ff.NumberOfFights >= 30 && ff.NumberOfFights < 40).ToList();
				else if (model.NumberOfFights == NumberOfFights.MoreThan40)
					fightersList = (fightersList == null) ? fighterRepo.GetAllFighters().Where(ff => ff.NumberOfFights >= 40).ToList() : fightersList.Where(ff => ff.NumberOfFights >= 40).ToList();

			}

			//filter by gender
			if (model.Sex != null)
			{
				if (fightersList == null)
					fightersList = fighterRepo.GetAllFighters().Where(ff => ff.Sex == model.Sex.Value).ToList();
				else
					fightersList = fightersList.Where(ff => ff.Sex == model.Sex.Value).ToList();
			}

			//filter by state
			if (String.IsNullOrEmpty(model.State) == false)
			{
				if (fightersList == null)
					fightersList = fighterRepo.GetAllFighters().Where(ff => ff.State == model.State).ToList();
				else
					fightersList = fightersList.Where(ff => ff.State == model.State).ToList();
			}

			if (fightersList == null)
				fightersList = fighterRepo.GetAllFighters();

			model.FightersList = getFightersListViewModel(fightersList);

			populateFilterDropdowns();

			return View(model);
		}

		public ActionResult About()
		{
			return View();
		}

		public ActionResult Contact()
		{
			return View();
		}

		[HttpPost]
		public ActionResult Contact(ContactViewModel model)
		{
			if (ModelState.IsValid == false)
				return View();

			string emailMessage = String.Format("Sender Name: {0}<br /><br />Message: <br /><br />{1}", model.Name, model.Message.Replace("\n", "<br />"));

			SparWeb.Util.SendEmail(model.Email, ConfigurationManager.AppSettings["EmailAdmin"], "SparGym Contact Form", emailMessage);
			ViewBag.SuccessMessage = "Thanks for sending your message. We'll get back to yiou you within 24 hours.";

			ModelState.Clear();

			return View();
		}

		private void populateFilterDropdowns()
		{
			ViewBag.AgeRange = Util.AgeRangeMap;
			ViewBag.WeightClassMap = Util.WeightClassMap;
			ViewBag.HeightToCentimetersMap = Util.HeightToCentimetersMap;
			ViewBag.NumberOfFights = Util.NumberOfFights;
			ViewBag.States = Util.States;
		}

		private List<AccountViewModel> getFightersListViewModel(IList<Fighter> fightersList)
		{
			FighterRepository fighterRepo = new FighterRepository();

			int loggedInFighterId = -1;
			if (User.Identity.GetUserId() != null)
				loggedInFighterId = fighterRepo.GetFighterByIdentityUserId(User.Identity.GetUserId()).Id.Value;

			List<AccountViewModel> fightersAccountViewModelList = new List<AccountViewModel>();
			foreach (Fighter currFighter in fightersList)
			{
				if (loggedInFighterId == -1 || loggedInFighterId != currFighter.Id)
				{
					AccountViewModel accountViewModel = Util.GetAccountViewModelForFighter(currFighter, 150);

					if (User.Identity.IsAuthenticated == true)
						accountViewModel.SparRequests = Util.GetSparRequestDetailsForFighter(currFighter.Id.Value, User.Identity.GetUserId()).Where(sr => sr.OpponentFighter.Id == currFighter.Id).ToList();

					fightersAccountViewModelList.Add(accountViewModel);
				}
			}

			return fightersAccountViewModelList;
		}

		private int getAge(DateTime dob)
		{
			DateTime now = DateTime.Today;

			int age = now.Year - dob.Year;
			if (dob > now.AddYears(-age)) age--;

			return age;
		}
	}
}