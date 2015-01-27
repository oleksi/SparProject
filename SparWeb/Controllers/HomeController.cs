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

			int loggedInFighterId = -1;
			if (User.Identity.GetUserId() != null)
				loggedInFighterId = fighterRepo.GetFighterByIdentityUserId(User.Identity.GetUserId()).Id.Value;

			IList<Fighter> fightersList = fighterRepo.GetAllFighters();

			List<AccountViewModel> fightersAccountViewModelList = new List<AccountViewModel>();
			foreach(Fighter currFighter in fightersList)
			{
				if (loggedInFighterId == -1 || loggedInFighterId != currFighter.Id)
				{
					AccountViewModel accountViewModel = Util.GetAccountViewModelForFighter(currFighter, 150);
					fightersAccountViewModelList.Add(accountViewModel);
				}
			}

			HomeViewModel model = new HomeViewModel();
			model.FightersList = fightersAccountViewModelList;

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
	}
}