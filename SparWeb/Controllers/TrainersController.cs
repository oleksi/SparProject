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
    public class TrainersController : Controller
    {
        // GET: Trainers
		public ActionResult Index(string state)
		{
			string stateShort = null;
			if (String.IsNullOrEmpty(state) == false)
				stateShort = Util.States.Where(ss => ss.Value.ToLower() == state.ToLower()).Select(ss => ss.Key).SingleOrDefault();

			var trainerRepo = new TrainerRepository();
			var trainers = trainerRepo.GetAllTrainers().ToList();

			if (String.IsNullOrEmpty(stateShort) == false)
				trainers = trainers.Where(gg => gg.State == stateShort).ToList();

			var trainersViewModelList = new List<AccountTrainerViewModel>();
			trainers.ForEach(tt => trainersViewModelList.Add(Util.GetAccountViewModelForTrainer(tt, 150)));

			var trainersViewModel = new TrainersViewModel() { TrainersList = trainersViewModelList, SearchState = stateShort, SearchStateLong = (String.IsNullOrEmpty(stateShort) == false) ? state : null };

			ViewBag.States = Util.States;

			return View(trainersViewModel);
		}

		public ActionResult Trainer(string state, string name)
		{
			string stateShort = Util.States.Where(ss => ss.Value.ToLower() == state.ToLower()).Select(ss => ss.Key).SingleOrDefault();
			if (String.IsNullOrEmpty(stateShort) == true)
				throw new ApplicationException("State is not valid!");

			var trainerRepo = new TrainerRepository();
			var trainer = trainerRepo.GetTrainerByStateAndName(stateShort, name);
			if (trainer == null)
				throw new ApplicationException("Trainer is not found!");

			var accountViewModel = Util.GetAccountViewModelForTrainer(trainer, 250);

			var fighterRepo = new FighterRepository();
			var fightersList = fighterRepo.GetAllFighters().Where(ff => ff.Trainer != null && ff.Trainer.Id == trainer.Id).ToList();
			var fightersAccountViewModelList = new List<AccountFighterViewModel>();
			foreach (var currFighter in fightersList)
			{
				var fighterAccountViewModel = Util.GetAccountViewModelForFighter(currFighter, 150);
				fightersAccountViewModelList.Add(fighterAccountViewModel);
			}
			accountViewModel.FightersList = fightersAccountViewModelList;

			Util.PopualateRegistrationDropdowns(ViewBag);

			return View(accountViewModel);
		}
    }
}
