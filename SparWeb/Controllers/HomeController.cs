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
using System.Configuration;
using System.Xml.Linq;
using reCAPTCHA.MVC;

namespace SparWeb.Controllers
{
	public class HomeController : Controller
	{
		[HttpGet]
		public ActionResult Index()
		{
			var model = new HomeViewModel();

			//var trainerRepo = new TrainerRepository();
			//model.FeaturedTrainersList = Util.GetRandomTrainersViewModels(2, null);
			
			////Gleason's is always the 1st
			//var gymRepo = new GymRepository();
			//var gleasonsGym = gymRepo.GetGymById(1);
			//var gleasonsGymViewModel = Util.GetGymViewModel(gleasonsGym, 150);
			//gleasonsGymViewModel.IsFeaturedMode = true;
			//model.FeaturedGymsList = Util.GetRandomGymsViewModels(1);
			//model.FeaturedGymsList.Insert(0, gleasonsGymViewModel);

			//model.FeaturedFightersList = Util.GetRandomFightersViewModels(User, 2);

			var sparRepo = new SparRepository();
			var fighterRepo = new FighterRepository();
			var sinceDate = DateTime.Now.AddDays(-14);
			var siteActivitiesViewModels = new List<SiteActivityViewModel>();

			var sparActivities = sparRepo.GetSparRequestActivitiesSince(sinceDate);
			foreach (var sparActivity in sparActivities)
			{
				var thisFighter = fighterRepo.GetFighterById(sparActivity.LastNegotiatorFighterId);
				var otherFighter = fighterRepo.GetFighterById((sparActivity.LastNegotiatorFighterId == sparActivity.RequestorFighterId)? sparActivity.OpponentFighterId : sparActivity.RequestorFighterId);
				var sparActivitiesViewModel = new SparActivityViewModel(Util.GetSparConfirmationViewModel(thisFighter, otherFighter, 150));
				sparActivitiesViewModel.SparRequesStatus = (SparRequestStatus)sparActivity.StatusId;
				sparActivitiesViewModel.ShowThisFighterUrl = true;
				var siteActivity = new SiteActivityViewModel() { SparActivity = sparActivitiesViewModel, ActivityDate = sparActivity.LastUpdateDate };
				siteActivitiesViewModels.Add(siteActivity);
			}

			var newFighters = fighterRepo.GetRegisterredFightersSince(sinceDate);
			foreach (var newFighter in newFighters)
			{
				var newFighterViewModel = Util.GetAccountViewModelForFighter(newFighter, 150);
				var siteActivity = new SiteActivityViewModel() { Fighter = newFighterViewModel, ActivityDate = newFighter.InsertDate };
				siteActivitiesViewModels.Add(siteActivity);
			}

			model.RecentActivities = siteActivitiesViewModels.OrderByDescending(sa => sa.ActivityDate).ToList();

			ViewBag.ShowPartners = true;

			return View(model);
		}

		public ActionResult About()
		{
			return View();
		}

		public ActionResult Contact()
		{
			return View();
		}

		[HttpPost]
		[CaptchaValidator]
		public ActionResult Contact(ContactViewModel model)
		{
			if (ModelState.IsValid == false)
				return View();

			var emailPlaceholders = new Dictionary<string, string>();
			emailPlaceholders["[NAME]"] = ConfigurationManager.AppSettings["AdminName"];
			emailPlaceholders["[SENDER_NAME]"] = model.Name;
			emailPlaceholders["[SENDER_EMAIL]"] = model.Email;
			emailPlaceholders["[MESSAGE]"] = model.Message.Replace("\n", "<br />");

			SparWeb.EmailManager.SendEmail(EmailManager.EmailTypes.ContactFormTemplate, ConfigurationManager.AppSettings["EmailSupport"], ConfigurationManager.AppSettings["EmailAdmin"], "Fightura Contact Form", emailPlaceholders);
			ViewBag.SuccessMessage = "Thanks for sending your message. We'll get back to yiou you within 24 hours.";

			ModelState.Clear();

			return View();
		}

		public ActionResult TermsAndConditions()
		{
			return View();
		}

		public ActionResult SiteMap()
		{
			XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";

			var urls = new List<string>();

			//adding static urls
			urls.Add(Url.Action("About", "Home"));
			urls.Add(Url.Action("Index", "Fighters"));
			
			urls.Add(Url.Action("Index", "Trainers"));
			foreach(var state in Util.States.Values)
				urls.Add(Url.Action("Index", "Trainers", new { state = state }));

			urls.Add(Url.Action("Index", "Gyms"));
			foreach (var state in Util.States.Values)
				urls.Add(Url.Action("Index", "Gyms", new { state = state }));

			urls.Add(Url.Action("Contact", "Home"));
			urls.Add(Url.Action("TermsAndConditions", "Home"));
			urls.Add(Url.Action("Regiser", "Account"));
			urls.Add(Url.Action("Login", "Account"));

			//adding trainer urls
			var trainerRepo = new TrainerRepository();
			var trainersList = trainerRepo.GetAllTrainers().ToList();
			var trainersListViewModels = Util.GetTrainersListViewModel(trainersList);
			trainersListViewModels.ForEach(ff => urls.Add(ff.TrainerUrl));

			//adding gyms urls
			var gymRepo = new GymRepository();
			var gymsList = gymRepo.GetAllGyms().ToList();
			var gymsListViewModels = Util.GetGymsListViewModel(gymsList);
			gymsListViewModels.ForEach(ff => urls.Add(ff.GymUrl));

			//adding fighter urls
			var fighterRepo = new FighterRepository();
			var fightersList = fighterRepo.GetAllFighters().Where(ff => ff.IsDemo == false).ToList();
			var fightersListViewModels = Util.GetFightersListViewModel(null, fightersList);
			fightersListViewModels.ForEach(ff => urls.Add(ff.FighterUrl));

			//building xml sitemap out of urls
			var sitemap = new XDocument(
				new XDeclaration("1.0", "utf-8", "yes"),
				new XElement(ns + "urlset",
				from url in urls
				select
				new XElement(ns + "url",
				new XElement(ns + "loc", String.Format("http://fightura.com{0}", url)),
				new XElement(ns + "lastmod", String.Format("{0:yyyy-MM-dd}", DateTime.Now)),
				new XElement(ns + "changefreq", "always"),
				new XElement(ns + "priority", "0.8")
			)));

			return Content("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n" + sitemap.ToString(), "text/xml");
		}

		public ActionResult Error()
		{
			return View("Error");
		}
	}
}