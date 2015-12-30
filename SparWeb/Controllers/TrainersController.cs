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
		const int PAGE_SIZE = 20;

        // GET: Trainers
		public ActionResult Index(string state, int? page)
		{
			string stateShort = null;
			if (String.IsNullOrEmpty(state) == false)
				stateShort = Util.States.Where(ss => ss.Value.ToLower() == state.ToLower()).Select(ss => ss.Key).SingleOrDefault();

			var trainerRepo = new TrainerRepository();
			var trainers = trainerRepo.GetAllTrainers().ToList();

			if (String.IsNullOrEmpty(stateShort) == false)
				trainers = trainers.Where(gg => gg.State == stateShort).ToList();

			int pageCount = (int)Math.Ceiling((Convert.ToDecimal(trainers.Count) / PAGE_SIZE));

			//pagination
			if (page.HasValue == true)
				trainers = trainers.Skip((page.Value - 1) * PAGE_SIZE).Take(PAGE_SIZE).ToList();
			else
				trainers = trainers.Take(PAGE_SIZE).ToList();

			var trainersViewModelList = Util.GetTrainersListViewModel(trainers);

			var trainersViewModel = new TrainersViewModel() { TrainersList = trainersViewModelList, SearchState = stateShort, SearchStateLong = (String.IsNullOrEmpty(stateShort) == false) ? state : null };
			trainersViewModel.PageNumber = page.HasValue ? page.Value : 1;
			trainersViewModel.PagesCount = pageCount;

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

			return View(accountViewModel);
		}
    }
}
