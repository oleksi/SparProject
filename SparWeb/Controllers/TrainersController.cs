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
    }
}
