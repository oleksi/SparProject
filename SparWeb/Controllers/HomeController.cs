using SparData;
using SparModel;
using SparWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SparWeb.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			FighterRepository fighterRepo = new FighterRepository();
			IList<Fighter> fightersList = fighterRepo.GetAllFighters();

			List<AccountViewModel> fightersAccountViewModelList = new List<AccountViewModel>();
			foreach( Fighter currFightiner in fightersList)
				fightersAccountViewModelList.Add(Util.GetAccountViewModelForFighter(currFightiner, 150));

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