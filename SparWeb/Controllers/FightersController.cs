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

namespace SparWeb.Controllers
{
    public class FightersController : Controller
    {
		const int PAGE_SIZE = 20;

		private bool IsDemoMode { get { return (Session["IsDemoMode"] != null && Convert.ToBoolean(Session["IsDemoMode"]) == true); } }

		[HttpGet]
		public ActionResult Index(FightersViewModel model, int? page)
		{
			if (Request.QueryString["demo"] != null && Request.QueryString["demo"] == "1")
				Session["IsDemoMode"] = true;

			FighterRepository fighterRepo = new FighterRepository();
			IList<Fighter> fightersList = null;

			//filter by age
			if (model.AgeRange != 0)
			{
				if (model.AgeRange == AgeRange.AgeBelow12)
					fightersList = fighterRepo.GetAllFighters().Where(ff => _getAge(ff.DateOfBirth) < 12).ToList();
				else if (model.AgeRange == AgeRange.Between12And16)
					fightersList = fighterRepo.GetAllFighters().Where(ff => _getAge(ff.DateOfBirth) >= 12 && _getAge(ff.DateOfBirth) < 16).ToList();
				else if (model.AgeRange == AgeRange.Between16And20)
					fightersList = fighterRepo.GetAllFighters().Where(ff => _getAge(ff.DateOfBirth) >= 16 && _getAge(ff.DateOfBirth) < 20).ToList();
				else if (model.AgeRange == AgeRange.Between20And25)
					fightersList = fighterRepo.GetAllFighters().Where(ff => _getAge(ff.DateOfBirth) >= 20 && _getAge(ff.DateOfBirth) < 25).ToList();
				else if (model.AgeRange == AgeRange.Between25And30)
					fightersList = fighterRepo.GetAllFighters().Where(ff => _getAge(ff.DateOfBirth) >= 25 && _getAge(ff.DateOfBirth) < 30).ToList();
				else if (model.AgeRange == AgeRange.Between30And35)
					fightersList = fighterRepo.GetAllFighters().Where(ff => _getAge(ff.DateOfBirth) >= 30 && _getAge(ff.DateOfBirth) < 35).ToList();
				else if (model.AgeRange == AgeRange.Between35And40)
					fightersList = fighterRepo.GetAllFighters().Where(ff => _getAge(ff.DateOfBirth) >= 35 && _getAge(ff.DateOfBirth) < 40).ToList();
				else if (model.AgeRange == AgeRange.Between40And45)
					fightersList = fighterRepo.GetAllFighters().Where(ff => _getAge(ff.DateOfBirth) >= 40 && _getAge(ff.DateOfBirth) < 45).ToList();
				else if (model.AgeRange == AgeRange.Between45And50)
					fightersList = fighterRepo.GetAllFighters().Where(ff => _getAge(ff.DateOfBirth) >= 45 && _getAge(ff.DateOfBirth) < 50).ToList();
				else if (model.AgeRange == AgeRange.Between50And55)
					fightersList = fighterRepo.GetAllFighters().Where(ff => _getAge(ff.DateOfBirth) >= 50 && _getAge(ff.DateOfBirth) < 55).ToList();
				else if (model.AgeRange == AgeRange.Between55And60)
					fightersList = fighterRepo.GetAllFighters().Where(ff => _getAge(ff.DateOfBirth) >= 55 && _getAge(ff.DateOfBirth) < 60).ToList();
				else if (model.AgeRange == AgeRange.Above60)
					fightersList = fighterRepo.GetAllFighters().Where(ff => _getAge(ff.DateOfBirth) >= 60).ToList();

				model.FilterParams.Add("AgeRange", Convert.ToInt32(model.AgeRange).ToString());
			}

			//filter by weight
			if (model.Weight != 0)
			{
				if (fightersList == null)
					fightersList = fighterRepo.GetAllFighters().Where(ff => ff.Weight == model.Weight).ToList();
				else
					fightersList = fightersList.Where(ff => ff.Weight == model.Weight).ToList();

				model.FilterParams.Add("Weight", model.Weight.ToString());
			}

			//filter by height
			if (model.Height != 0)
			{
				if (fightersList == null)
					fightersList = fighterRepo.GetAllFighters().Where(ff => ff.Height == model.Height).ToList();
				else
					fightersList = fightersList.Where(ff => ff.Height == model.Height).ToList();

				model.FilterParams.Add("Height", model.Height.ToString());
			}

			//filter by stance
			if (model.Southpaw != null)
			{
				if (fightersList == null)
					fightersList = fighterRepo.GetAllFighters().Where(ff => ff.IsSouthpaw == model.Southpaw.Value).ToList();
				else
					fightersList = fightersList.Where(ff => ff.IsSouthpaw == model.Southpaw.Value).ToList();

				model.FilterParams.Add("Southpaw", model.Southpaw.Value.ToString());
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

				model.FilterParams.Add("NumberOfFights", Convert.ToInt32(model.NumberOfFights).ToString());
			}

			//filter by gender
			if (model.Male != null)
			{
				if (fightersList == null)
					fightersList = fighterRepo.GetAllFighters().Where(ff => ff.Sex == model.Male.Value).ToList();
				else
					fightersList = fightersList.Where(ff => ff.Sex == model.Male.Value).ToList();

				model.FilterParams.Add("Male", model.Male.ToString());
			}

			//filter by state
			if (String.IsNullOrEmpty(model.State) == false)
			{
				if (fightersList == null)
					fightersList = fighterRepo.GetAllFighters().Where(ff => ff.State == model.State).ToList();
				else
					fightersList = fightersList.Where(ff => ff.State == model.State).ToList();

				model.FilterParams.Add("State", model.State);
			}

			if (fightersList == null)
				fightersList = fighterRepo.GetAllFighters();

			if (IsDemoMode == false)
				fightersList = fightersList.Where(ff => ff.IsDemo == false).ToList();

			int pageCount = (int)Math.Ceiling((Convert.ToDecimal(fightersList.Count) / PAGE_SIZE));

			//pagination
			if (page.HasValue == true)
				fightersList = fightersList.Skip((page.Value - 1) * PAGE_SIZE).Take(PAGE_SIZE).ToList();
			else
				fightersList = fightersList.Take(PAGE_SIZE).ToList();

			model.FightersList = Util.GetFightersListViewModel(User, fightersList);
			model.PageNumber = page.HasValue ? page.Value : 1;
			model.PagesCount = pageCount;

			Util.PopulateFilterDropdowns(ViewBag);

			ViewBag.ShowPartners = true;

			return View(model);
		}

		public ActionResult Fighter(string state, string name)
		{
			string stateShort = Util.States.Where(ss => ss.Value.ToLower() == state.ToLower()).Select(ss => ss.Key).SingleOrDefault();
			if (String.IsNullOrEmpty(stateShort) == true)
				throw new ApplicationException("State is not valid!");

			var fighterRepo = new FighterRepository();
			var fighter = fighterRepo.GetFighterByStateAndName(stateShort, name);
			if (fighter == null)
				throw new ApplicationException("Fighter is not found!");

			var accountViewModel = Util.GetAccountViewModelForFighter(fighter, 250);
			if (User.Identity.IsAuthenticated == true)
			{
				var relatedFightersList = Util.GetRelatedFightersList(User.Identity.GetUserId());
				var sparRequestDetails = Util.GetSparRequestDetailsForFighter(fighter.Id.Value, User.Identity.GetUserId());
				accountViewModel.SparRequests = sparRequestDetails.Where(sr => (relatedFightersList.Contains(sr.OpponentFighter.Id.Value) == true || relatedFightersList.Contains(sr.ThisFighter.Id.Value) == true)).ToList();
			}

			Util.PopualateRegistrationDropdowns(ViewBag);

			return View(accountViewModel);
		}

		private int _getAge(DateTime dob)
		{
			DateTime now = DateTime.Today;

			int age = now.Year - dob.Year;
			if (dob > now.AddYears(-age)) age--;

			return age;
		}
    }
}