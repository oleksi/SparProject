using SparData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SparWeb.Models;
using Microsoft.AspNet.Identity.Owin;

namespace SparWeb.Controllers
{
    public class PartnershipController : Controller
    {
        public ActionResult RandomFightersWidget(int fightersNum, string backgroundColor, string foregroundColor)
        {
			var fighterRepo = new FighterRepository();
			var fightersList = fighterRepo.GetFightersWithProfilePics();

			//getting random number of fighters
			fightersList.Shuffle();
			var randomFightersList = fightersList.Take(fightersNum).ToList();

			var model = new RandomFightersWidgetViewModel()
			{
				BackgroundColor = backgroundColor,
				ForegroundColor = foregroundColor,
				FightersList = Util.GetFightersListViewModel(User, randomFightersList)
			};

			//letting view know that this is the Widget view, so that we don't show Spar Him/Her button
			model.FightersList.ForEach(ff => ff.IsWidgetView = true);

			return View(model);
        }
    }
}