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
			model.Sex = true;

			ViewBag.AgeRange = Util.AgeRangeMap;
			ViewBag.WeightClassMap = Util.WeightClassMap;
			ViewBag.HeightToCentimetersMap = Util.HeightToCentimetersMap;

			return View(model);
		}

		[HttpPost]
		public ActionResult Index(HomeViewModel model)
		{
			FighterRepository fighterRepo = new FighterRepository();
			IList<Fighter> fightersList = null;

			if (model.AgeRange != null)
			{
				if (model.AgeRange == AgeRange.AgeBelow12)
					fightersList = fighterRepo.GetAllFighters().Where(ff => getAge(ff.DateOfBirth) < 12).ToList();
				else if (model.AgeRange == AgeRange.Between12And16)
					fightersList = fighterRepo.GetAllFighters().Where(ff => getAge(ff.DateOfBirth) >= 12 && getAge(ff.DateOfBirth) < 16).ToList();
				else if (model.AgeRange == AgeRange.Between16And20)
					fightersList = fighterRepo.GetAllFighters().Where(ff => getAge(ff.DateOfBirth) >= 16 && getAge(ff.DateOfBirth) < 20).ToList();
				else if (model.AgeRange == AgeRange.Between20And24)
					fightersList = fighterRepo.GetAllFighters().Where(ff => getAge(ff.DateOfBirth) >= 20 && getAge(ff.DateOfBirth) < 24).ToList();
				else if (model.AgeRange == AgeRange.Between24And30)
					fightersList = fighterRepo.GetAllFighters().Where(ff => getAge(ff.DateOfBirth) >= 24 && getAge(ff.DateOfBirth) < 30).ToList();
				else if (model.AgeRange == AgeRange.Between30And36)
					fightersList = fighterRepo.GetAllFighters().Where(ff => getAge(ff.DateOfBirth) >= 30 && getAge(ff.DateOfBirth) < 36).ToList();
				else if (model.AgeRange == AgeRange.Above36)
					fightersList = fighterRepo.GetAllFighters().Where(ff => getAge(ff.DateOfBirth) >= 36).ToList();
			}

			if (fightersList == null)
				fightersList = fighterRepo.GetAllFighters();

			model.FightersList = getFightersListViewModel(fightersList);

			ViewBag.AgeRange = Util.AgeRangeMap;
			ViewBag.WeightClassMap = Util.WeightClassMap;
			ViewBag.HeightToCentimetersMap = Util.HeightToCentimetersMap;

			return View(model);
		}

		public ActionResult About()
		{
			ViewBag.Message = "Your application description page.";

			return View();
		}

		public ActionResult Contact()
		{
			ViewBag.Message = "Your contact page.";

			return View();
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