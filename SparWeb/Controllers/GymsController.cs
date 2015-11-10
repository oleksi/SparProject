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
        public ActionResult Index()
        {
			var gymRepo = new GymRepository();
			var gyms = gymRepo.GetAllGyms().ToList();

			var gymsViewModelList = new List<GymViewModel>();
			gyms.ForEach(gg => gymsViewModelList.Add(new GymViewModel() { Id = gg.Id.Value , Name = gg.Name, StreetAddress = gg.StreetAddress, City = gg.City, State = gg.State, ZipCode = gg.ZipCode, Phone = gg.Phone, GymPictureFile = Util.GetGymPictureFile(gg, 150) }));

			var gymsViewModel = new GymsViewModel() { GymsList = gymsViewModelList };

			ViewBag.States = Util.States;

			return View(gymsViewModel);
        }
    }
}