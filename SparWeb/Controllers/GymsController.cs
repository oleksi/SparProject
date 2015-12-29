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
    public class GymsController : Controller
    {
		const int PAGE_SIZE = 20;

        // GET: Gyms
		public ActionResult Index(string state, int? page)
        {
			string stateShort = null;
			if (String.IsNullOrEmpty(state) == false)
				stateShort = Util.States.Where(ss => ss.Value.ToLower() == state.ToLower()).Select(ss => ss.Key).SingleOrDefault();

			var gymRepo = new GymRepository();
			var gyms = gymRepo.GetAllGyms().ToList();

			if (String.IsNullOrEmpty(stateShort) == false)
				gyms = gyms.Where(gg => gg.State == stateShort).ToList();

			int pageCount = (int)Math.Ceiling((Convert.ToDecimal(gyms.Count) / PAGE_SIZE));

			//pagination
			if (page.HasValue == true)
				gyms = gyms.Skip((page.Value - 1) * PAGE_SIZE).Take(PAGE_SIZE).ToList();
			else
				gyms = gyms.Take(PAGE_SIZE).ToList();

			var gymsViewModelList = Util.GetGymsListViewModel(gyms);
			var gymsViewModel = new GymsViewModel() { GymsList = gymsViewModelList, SearchState = stateShort, SearchStateLong = (String.IsNullOrEmpty(stateShort) == false)? state : null };
			gymsViewModel.PageNumber = page.HasValue ? page.Value : 1;
			gymsViewModel.PagesCount = pageCount;

			ViewBag.States = Util.States;

			return View(gymsViewModel);
        }

		public ActionResult Gym(string state, string name)
		{
			string stateShort = Util.States.Where(ss => ss.Value.ToLower() == state.ToLower()).Select(ss => ss.Key).SingleOrDefault();
			if (String.IsNullOrEmpty(stateShort) == true)
				throw new ApplicationException("State is not valid!");

			var gymRepo = new GymRepository();
			var gym = gymRepo.GetGymByStateAndName(stateShort, name);
			if (gym == null)
				throw new ApplicationException("Gym is not found!");

			var gymViewModel = Util.GetGymViewModel(gym, 250);

			var trainerRepo = new TrainerRepository();
			var trainers = trainerRepo.GetAllTrainers().Where(tt => tt.Gym != null && tt.Gym.Id == gym.Id).ToList();
			var trainersViewModelList = new List<AccountTrainerViewModel>();
			trainers.ForEach(tt => trainersViewModelList.Add(Util.GetAccountViewModelForTrainer(tt, 150)));
			gymViewModel.TrainersList = trainersViewModelList;

			var fighterRepo = new FighterRepository();
			var fightersList = fighterRepo.GetAllFighters().Where(ff => ff.Gym != null && ff.Gym.Id == gym.Id).ToList();
			var fightersAccountViewModelList = new List<AccountFighterViewModel>();
			foreach (var currFighter in fightersList)
			{
				var fighterAccountViewModel = Util.GetAccountViewModelForFighter(currFighter, 150);
				fightersAccountViewModelList.Add(fighterAccountViewModel);
			}
			gymViewModel.FightersList = fightersAccountViewModelList;

			Util.PopualateRegistrationDropdowns(ViewBag);

			return View(gymViewModel);
		}
    }
}