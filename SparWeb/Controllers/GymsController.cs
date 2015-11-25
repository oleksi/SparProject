using SparData;
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
			gyms.ForEach(gg => gymsViewModelList.Add(new GymViewModel() { Id = gg.Id.Value , Name = gg.Name, StreetAddress = gg.StreetAddress, City = gg.City, State = gg.State, ZipCode = gg.ZipCode, Phone = gg.Phone, GymPictureFile = Util.GetGymPictureFile(gg, 150) }));

			var gymsViewModel = new GymsViewModel() { GymsList = gymsViewModelList, SearchState = stateShort, SearchStateLong = (String.IsNullOrEmpty(stateShort) == false)? state : null };

			ViewBag.States = Util.States;

			return View(gymsViewModel);
        }
    }
}