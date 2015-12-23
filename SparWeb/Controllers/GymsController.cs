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
        // GET: Gyms
        public ActionResult Index(string state)
        {
			string stateShort = null;
			if (String.IsNullOrEmpty(state) == false)
				stateShort = Util.States.Where(ss => ss.Value.ToLower() == state.ToLower()).Select(ss => ss.Key).SingleOrDefault();

			var gymRepo = new GymRepository();
			var gyms = gymRepo.GetAllGyms().ToList();

			if (String.IsNullOrEmpty(stateShort) == false)
				gyms = gyms.Where(gg => gg.State == stateShort).ToList();

			var gymsViewModelList = new List<GymViewModel>();
			gyms.ForEach(gg => gymsViewModelList.Add(_getGymViewModel(gg, 150)));

			var gymsViewModel = new GymsViewModel() { GymsList = gymsViewModelList, SearchState = stateShort, SearchStateLong = (String.IsNullOrEmpty(stateShort) == false)? state : null };

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
				throw new ApplicationException("Fighter is not found!");

			var gymViewModel = _getGymViewModel(gym, 250);
			
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

		private GymViewModel _getGymViewModel(Gym gym, int thumbnailSize)
		{
			return new GymViewModel() { Id = gym.Id.Value, Name = gym.Name, StreetAddress = gym.StreetAddress, City = gym.City, State = gym.State, ZipCode = gym.ZipCode, Phone = gym.Phone, GymPictureFile = Util.GetGymPictureFile(gym, thumbnailSize) };
		}
    }
}