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

namespace SparWeb.Controllers
{
	public class HomeController : Controller
	{
		const int PAGE_SIZE = 20;

		[HttpGet]
		public ActionResult Index(HomeViewModel model, int? page)
		{
			FighterRepository fighterRepo = new FighterRepository();
			IList<Fighter> fightersList = null;

			//filter by age
			if (model.AgeRange != 0)
			{
				if (model.AgeRange == AgeRange.AgeBelow12)
					fightersList = fighterRepo.GetAllFighters().Where(ff => getAge(ff.DateOfBirth) < 12).ToList();
				else if (model.AgeRange == AgeRange.Between12And16)
					fightersList = fighterRepo.GetAllFighters().Where(ff => getAge(ff.DateOfBirth) >= 12 && getAge(ff.DateOfBirth) < 16).ToList();
				else if (model.AgeRange == AgeRange.Between16And20)
					fightersList = fighterRepo.GetAllFighters().Where(ff => getAge(ff.DateOfBirth) >= 16 && getAge(ff.DateOfBirth) < 20).ToList();
				else if (model.AgeRange == AgeRange.Between20And25)
					fightersList = fighterRepo.GetAllFighters().Where(ff => getAge(ff.DateOfBirth) >= 20 && getAge(ff.DateOfBirth) < 25).ToList();
				else if (model.AgeRange == AgeRange.Between25And30)
					fightersList = fighterRepo.GetAllFighters().Where(ff => getAge(ff.DateOfBirth) >= 25 && getAge(ff.DateOfBirth) < 30).ToList();
				else if (model.AgeRange == AgeRange.Between30And35)
					fightersList = fighterRepo.GetAllFighters().Where(ff => getAge(ff.DateOfBirth) >= 30 && getAge(ff.DateOfBirth) < 35).ToList();
				else if (model.AgeRange == AgeRange.Between35And40)
					fightersList = fighterRepo.GetAllFighters().Where(ff => getAge(ff.DateOfBirth) >= 35 && getAge(ff.DateOfBirth) < 40).ToList();
				else if (model.AgeRange == AgeRange.Between40And45)
					fightersList = fighterRepo.GetAllFighters().Where(ff => getAge(ff.DateOfBirth) >= 40 && getAge(ff.DateOfBirth) < 45).ToList();
				else if (model.AgeRange == AgeRange.Between45And50)
					fightersList = fighterRepo.GetAllFighters().Where(ff => getAge(ff.DateOfBirth) >= 45 && getAge(ff.DateOfBirth) < 50).ToList();
				else if (model.AgeRange == AgeRange.Between50And55)
					fightersList = fighterRepo.GetAllFighters().Where(ff => getAge(ff.DateOfBirth) >= 50 && getAge(ff.DateOfBirth) < 55).ToList();
				else if (model.AgeRange == AgeRange.Between55And60)
					fightersList = fighterRepo.GetAllFighters().Where(ff => getAge(ff.DateOfBirth) >= 55 && getAge(ff.DateOfBirth) < 60).ToList();
				else if (model.AgeRange == AgeRange.Above60)
					fightersList = fighterRepo.GetAllFighters().Where(ff => getAge(ff.DateOfBirth) >= 60).ToList();

				model.FilterParams.Add("AgeRange", Convert.ToInt32(model.AgeRange).ToString());
			}

			//filter by weight
			if (model.Weight != 0)
			{
				if (fightersList == null)
					fightersList = fighterRepo.GetAllFighters().Where(ff => ff.Weight == model.Weight).ToList();
				else
					fightersList = fightersList.Where(ff => ff.Weight == model.Weight).ToList();

				model.FilterParams.Add("Weight", model.Weight.ToString());
			}

			//filter by height
			if (model.Height != 0)
			{
				if (fightersList == null)
					fightersList = fighterRepo.GetAllFighters().Where(ff => ff.Height == model.Height).ToList();
				else
					fightersList = fightersList.Where(ff => ff.Height == model.Height).ToList();

				model.FilterParams.Add("Height", model.Height.ToString());
			}

			//filter by stance
			if (model.Southpaw != null)
			{
				if (fightersList == null)
					fightersList = fighterRepo.GetAllFighters().Where(ff => ff.IsSouthpaw == model.Southpaw.Value).ToList();
				else
					fightersList = fightersList.Where(ff => ff.IsSouthpaw == model.Southpaw.Value).ToList();

				model.FilterParams.Add("Southpaw", model.Southpaw.Value.ToString());
			}

			if (model.NumberOfFights != 0)
			{
				if (model.NumberOfFights == NumberOfFights.NoFights)
					fightersList = (fightersList == null) ? fighterRepo.GetAllFighters().Where(ff => ff.NumberOfFights == 0).ToList() : fightersList.Where(ff => ff.NumberOfFights == 0).ToList();
				else if (model.NumberOfFights == NumberOfFights.Between1And5Fights)
					fightersList = (fightersList == null) ? fighterRepo.GetAllFighters().Where(ff => ff.NumberOfFights > 0 && ff.NumberOfFights < 5).ToList() : fightersList.Where(ff => ff.NumberOfFights > 0 && ff.NumberOfFights < 5).ToList();
				else if (model.NumberOfFights == NumberOfFights.Betwee5nAnd10Fights)
					fightersList = (fightersList == null) ? fighterRepo.GetAllFighters().Where(ff => ff.NumberOfFights >= 5 && ff.NumberOfFights < 10).ToList() : fightersList.Where(ff => ff.NumberOfFights >= 5 && ff.NumberOfFights < 10).ToList();
				else if (model.NumberOfFights == NumberOfFights.Between10And15Fights)
					fightersList = (fightersList == null) ? fighterRepo.GetAllFighters().Where(ff => ff.NumberOfFights >= 10 && ff.NumberOfFights < 15).ToList() : fightersList.Where(ff => ff.NumberOfFights >= 10 && ff.NumberOfFights < 15).ToList();
				else if (model.NumberOfFights == NumberOfFights.Between15And20Fights)
					fightersList = (fightersList == null) ? fighterRepo.GetAllFighters().Where(ff => ff.NumberOfFights >= 15 && ff.NumberOfFights < 20).ToList() : fightersList.Where(ff => ff.NumberOfFights >= 15 && ff.NumberOfFights < 20).ToList();
				else if (model.NumberOfFights == NumberOfFights.Between20And30Fights)
					fightersList = (fightersList == null) ? fighterRepo.GetAllFighters().Where(ff => ff.NumberOfFights >= 20 && ff.NumberOfFights < 30).ToList() : fightersList.Where(ff => ff.NumberOfFights >= 20 && ff.NumberOfFights < 30).ToList();
				else if (model.NumberOfFights == NumberOfFights.Between30And40Fights)
					fightersList = (fightersList == null) ? fighterRepo.GetAllFighters().Where(ff => ff.NumberOfFights >= 30 && ff.NumberOfFights < 40).ToList() : fightersList.Where(ff => ff.NumberOfFights >= 30 && ff.NumberOfFights < 40).ToList();
				else if (model.NumberOfFights == NumberOfFights.MoreThan40)
					fightersList = (fightersList == null) ? fighterRepo.GetAllFighters().Where(ff => ff.NumberOfFights >= 40).ToList() : fightersList.Where(ff => ff.NumberOfFights >= 40).ToList();

				model.FilterParams.Add("NumberOfFights", Convert.ToInt32(model.NumberOfFights).ToString());
			}

			//filter by gender
			if (model.Male != null)
			{
				if (fightersList == null)
					fightersList = fighterRepo.GetAllFighters().Where(ff => ff.Sex == model.Male.Value).ToList();
				else
					fightersList = fightersList.Where(ff => ff.Sex == model.Male.Value).ToList();

				model.FilterParams.Add("Male", model.Male.ToString());
			}

			//filter by state
			if (String.IsNullOrEmpty(model.State) == false)
			{
				if (fightersList == null)
					fightersList = fighterRepo.GetAllFighters().Where(ff => ff.State == model.State).ToList();
				else
					fightersList = fightersList.Where(ff => ff.State == model.State).ToList();

				model.FilterParams.Add("State", model.State);
			}

			if (fightersList == null)
				fightersList = fighterRepo.GetAllFighters();

			int pageCount = (int)Math.Ceiling((Convert.ToDecimal(fightersList.Count) / PAGE_SIZE));

			//pagination
			if (page.HasValue == true)
				fightersList = fightersList.Skip((page.Value - 1) * PAGE_SIZE).Take(PAGE_SIZE).ToList();
			else
				fightersList = fightersList.Take(PAGE_SIZE).ToList();

			model.FightersList = Util.GetFightersListViewModel(User, fightersList);
			model.PageNumber = page.HasValue ? page.Value : 1;
			model.PagesCount = pageCount;

			populateFilterDropdowns();

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
		public ActionResult Contact(ContactViewModel model)
		{
			if (ModelState.IsValid == false)
				return View();

			var emailPlaceholders = new Dictionary<string, string>();
			emailPlaceholders["[NAME]"] = ConfigurationManager.AppSettings["AdminName"];
			emailPlaceholders["[SENDER_NAME]"] = model.Name;
			emailPlaceholders["[SENDER_EMAIL]"] = model.Email;
			emailPlaceholders["[MESSAGE]"] = model.Message.Replace("\n", "<br />");

			SparWeb.EmailManager.SendEmail(EmailManager.EmailTypes.ContactFormTemplate, ConfigurationManager.AppSettings["EmailSupport"], ConfigurationManager.AppSettings["EmailAdmin"], "SparGym Contact Form", emailPlaceholders);
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
			urls.Add(Url.Action("Contact", "Home"));
			urls.Add(Url.Action("TermsAndConditions", "Home"));
			urls.Add(Url.Action("Regiser", "Account"));
			urls.Add(Url.Action("Login", "Account"));

			//adding fighter urls
			var fighterRepo = new FighterRepository();
			var fightersList = fighterRepo.GetAllFighters();
			var fightersListViewModels = Util.GetFightersListViewModel(null, fightersList);
			fightersListViewModels.ForEach(ff => urls.Add(ff.FighterUrl));

			//building xml sitemap out of urls
			var sitemap = new XDocument(
				new XDeclaration("1.0", "utf-8", "yes"),
				new XElement(ns + "urlset",
				from url in urls
				select
				new XElement(ns + "url",
				new XElement(ns + "loc", String.Format("http://spargym.com{0}", url)),
				new XElement(ns + "lastmod", String.Format("{0:yyyy-MM-dd}", DateTime.Now)),
				new XElement(ns + "changefreq", "always"),
				new XElement(ns + "priority", "0.8")
			)));

			return Content("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n" + sitemap.ToString(), "text/xml");
		}

		private void populateFilterDropdowns()
		{
			ViewBag.AgeRange = Util.AgeRangeMap;
			ViewBag.WeightClassMap = Util.WeightClassMap;
			ViewBag.HeightToCentimetersMap = Util.HeightToCentimetersMap;
			ViewBag.NumberOfFights = Util.NumberOfFights;
			ViewBag.States = Util.States;
		}

		private int getAge(DateTime dob)
		{
			DateTime now = DateTime.Today;

			int age = now.Year - dob.Year;
			if (dob > now.AddYears(-age)) age--;

			return age;
		}
	}
}