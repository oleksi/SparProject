using SparData;
using SparModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using SparWeb.Models;
using System.Configuration;

namespace SparWeb.Controllers
{
    public class SparController : Controller
    {
        //
        // GET: /Fighter/
        public ActionResult Index()
        {
            return View();
        }

		[Authorize]
		public ActionResult SparConfirmation(string ID)
		{
			FighterRepository fighterRepo = new FighterRepository();

			int loggedInFighterId = -1;
			if (User.Identity.GetUserId() != null)
				loggedInFighterId = fighterRepo.GetFighterByIdentityUserId(User.Identity.GetUserId()).Id.Value;

			int opponentFighterId = fighterRepo.GetFighterByIdentityUserId(ID).Id.Value;

			//checking if current user has already requested, negotiated or confirmed the spar with this fighter and if so not letting to request another one
			bool alreadyRequestedSpar = Util.GetSparRequestDetailsForFighter(opponentFighterId, User.Identity.GetUserId()).Any(sr => (sr.ThisFighter.Id == loggedInFighterId || sr.OpponentFighter.Id == loggedInFighterId));
			if (alreadyRequestedSpar == true)
				return RedirectToAction("Index", "Home");

			SparConfirmationViewModel sparConfirmationViewModel = getSparConfirmationViewModelForOpponent(ID);

			return View(sparConfirmationViewModel);
		}

		[Authorize]
		[HttpPost]
		public ActionResult ConfirmSpar(string OpponentId)
		{
			FighterRepository fighterRepo = new FighterRepository();
			Fighter thisFighter = fighterRepo.GetFighterByIdentityUserId(User.Identity.GetUserId());
			Fighter opponentFighter = fighterRepo.GetFighterByIdentityUserId(OpponentId);

			SparRequest sparRequest = new SparRequest()
			{
				OpponentFighter = opponentFighter,
				RequestDate = DateTime.Now,
				RequestorFighter = thisFighter,
				Status = SparRequestStatus.Requested,
				LastNegotiatorFighterId = thisFighter.Id.Value
			};

			//saving spar request
			SparRepository sparRepo = new SparRepository();
			sparRepo.CreateSparRequest(sparRequest);

			//sending out notification email
			String emailTo = opponentFighter.SparIdentityUser.Email;
			String emailSubject = String.Format("{0} wants to spar you!", thisFighter.Name);
			Dictionary<string, string> emailPlaceholders = new Dictionary<string, string>();
			emailPlaceholders["[NAME]"] = opponentFighter.Name;
			emailPlaceholders["[FIGTHER_NAME]"] = thisFighter.Name;
			emailPlaceholders["[GENDER]"] = (thisFighter.Sex == true) ? "Male" : "Femail";
			emailPlaceholders["[DATE_OF_BIRTH]"] = thisFighter.DateOfBirth.ToShortDateString();
			emailPlaceholders["[CITY]"] = thisFighter.City;
			emailPlaceholders["[STATE]"] = thisFighter.State;
			emailPlaceholders["[HEIGHT]"] = Util.HeightToCentimetersMap[thisFighter.Height];
			emailPlaceholders["[WEIGHT]"] = Util.WeightClassMap[thisFighter.Weight];
			emailPlaceholders["[STANCE]"] = (thisFighter.IsSouthpaw) ? "Left-handed" : "Right-handed";
			emailPlaceholders["[NUMBER_OF_AMATEUR_FIGHTS]"] = thisFighter.NumberOfAmateurFights.ToString();
			emailPlaceholders["[NUMBER_OF_PRO_FUIGTS]"] = thisFighter.NumberOfProFights.ToString();
			emailPlaceholders["[GYM]"] = (thisFighter.Gym == null) ? "Unknown Gym" : thisFighter.Gym.Name;
			emailPlaceholders["[SPAR_CONFIRMATION_URL]"] = Url.Action("SparDetailsConfirmation", "Spar", new System.Web.Routing.RouteValueDictionary() { { "ID", sparRequest.Id } }, "http", Request.Url.Host);

			SparWeb.EmailManager.SendEmail(EmailManager.EmailTypes.SparRequestInitialTemplate, ConfigurationManager.AppSettings["EmailSupport"], emailTo, emailSubject, emailPlaceholders);

			SparConfirmationViewModel sparConfirmationViewModel = getSparConfirmationViewModelForOpponent(OpponentId);
			return View("SparConfirmed", sparConfirmationViewModel);
		}

		[Authorize]
		public ActionResult SparDetailsConfirmation(string ID)
		{
			SparRepository sparRepo = new SparRepository();
			SparRequest sparRequest = sparRepo.GetSparRequestById(ID);

			ConfirmSparDetailsViewModel confirmSparDetailsViewModel = Util.GetConfirmSparDetailsViewModel(sparRequest, 250, User.Identity.GetUserId()); 

			return View(confirmSparDetailsViewModel);
		}
		
		[Authorize]
		[HttpPost]
		public ActionResult ChangeSparDetails(ConfirmSparDetailsViewModel model)
		{
			SparRepository sparRepo = new SparRepository();
			SparRequest sparRequest = sparRepo.GetSparRequestById(model.SparRequestId);

			ConfirmSparDetailsViewModel confirmSparDetailsViewModel = Util.GetConfirmSparDetailsViewModel(sparRequest, 250, User.Identity.GetUserId());
			confirmSparDetailsViewModel.SparDate = model.SparDate;
			confirmSparDetailsViewModel.SparTime = model.SparTime;
			
			confirmSparDetailsViewModel.SparGymID = model.SparGymID;
			if (model.SparGymID > 0)
			{
				GymRepository gymRepo = new GymRepository();
				confirmSparDetailsViewModel.SparGym = gymRepo.GetGymById(model.SparGymID);
			}
			
			confirmSparDetailsViewModel.SparNotes = model.SparNotes;

			if (ModelState.IsValid == false || model.SparDate <= DateTime.Now)
			{
				if (model.SparDate <= DateTime.Now)
					ModelState.AddModelError("SparDate", "Spar Date must be in the future");

				return View("SparDetailsConfirmation", confirmSparDetailsViewModel);
			}

			confirmSparDetailsViewModel.SparRequesStatus = model.SparRequesStatus;

			//checking if it's 1st response to the spar request
			bool isFirstResponse = (sparRequest.Status == SparRequestStatus.Requested);

			//updating spar request
			if (model.SparRequesStatus == SparRequestStatus.DateLocationNegotiation)
			{
				sparRequest.SparDateTime = DateTime.Parse(String.Format("{0} {1}", model.SparDate.Value.ToString("MM/dd/yyyy"), model.SparTime.ToString()));
				if (model.SparGymID > 0)
					sparRequest.SparGym = confirmSparDetailsViewModel.SparGym;
				sparRequest.SparNotes = model.SparNotes;
			}

			FighterRepository fighterRepo = new FighterRepository();
			Fighter thisFighter = fighterRepo.GetFighterByIdentityUserId(User.Identity.GetUserId());
			sparRequest.LastNegotiatorFighterId = thisFighter.Id.Value;

			sparRequest.Status = model.SparRequesStatus;

			sparRepo.SaveSparRequest(sparRequest);

			//send email to spar requestor
			sendEmailForSaprRequest(confirmSparDetailsViewModel, isFirstResponse);

			return View("SparDetailsConfirmed", confirmSparDetailsViewModel);
		}

		[Authorize]
		[HttpPost]
		public ActionResult CancelSpar(string sparRequestId)
		{
			var sparRepo = new SparRepository();
			var sparRequest = sparRepo.GetSparRequestById(sparRequestId);

			sparRequest.Status = SparRequestStatus.Canceled;
			sparRepo.SaveSparRequest(sparRequest);

			//sending confirmation email
			ConfirmSparDetailsViewModel confirmSparDetailsViewModel = Util.GetConfirmSparDetailsViewModel(sparRequest, 250, User.Identity.GetUserId());
			sendEmailForSaprRequest(confirmSparDetailsViewModel, false);

			return new JsonResult() { Data = new { Result = "Ok" } };
		}

		private void sendEmailForSaprRequest(ConfirmSparDetailsViewModel model, bool isFirstResponse)
		{
			string emailTo = model.OpponentFighter.SparIdentityUser.Email;

			string emailSubject = "";
			SparWeb.EmailManager.EmailTypes emailType = 0;

			var emailPlaceholoders = new Dictionary<string, string>();
			emailPlaceholoders["[NAME]"] = model.OpponentFighter.Name;
			emailPlaceholoders["[SPAR_DATE]"] = model.SparDate.Value.ToString("MM/dd/yyyy");
			emailPlaceholoders["[SPAR_TIME]"] = model.SparTime.ToString();
			emailPlaceholoders["[SPAR_LOCATION]"] = (model.SparGymID > 0) ? model.SparGym.Name : "N/A";
			emailPlaceholoders["[SPAR_NOTES]"] = (String.IsNullOrEmpty(model.SparNotes) == false) ? String.Format("Notes: {0}<br />", model.SparNotes) : "";
				
			if (isFirstResponse == true) // first response
			{
				emailPlaceholoders["[FIGTHER_NAME]"] = model.ThisFighter.Name;
				emailPlaceholoders["[HE_OR_SHE]"] = model.ThisFighter.GetHeOrShe(true);
				emailSubject = String.Format("{0} has confirmed your spar request", model.ThisFighter.Name);
				emailType = EmailManager.EmailTypes.SparRequestFirstTimeResponseTemplate;
			}
			else if (model.SparRequesStatus == SparRequestStatus.DateLocationNegotiation) // spar negotiation
			{
				emailSubject = String.Format("{0} updated spar details", model.ThisFighter.Name);
				emailPlaceholoders["[FIGTHER_NAME]"] = model.ThisFighter.Name;
				emailType = EmailManager.EmailTypes.SparRequestNegotiationTemplate;
			}
			else if (model.SparRequesStatus == SparRequestStatus.Confirmed) // spar is confirmed
			{
				emailSubject = String.Format("{0} has confirmed the spar", model.ThisFighter.Name);
				emailPlaceholoders["[FIGTHER_NAME]"] = model.ThisFighter.Name;
				emailType = EmailManager.EmailTypes.SparRequestConfirmedTemplate;
			}
			else if (model.SparRequesStatus == SparRequestStatus.Canceled) // spar is canceled
			{
				emailSubject = String.Format("{0} has canceled the spar", model.ThisFighter.Name);
				emailPlaceholoders["[FIGTHER_NAME]"] = model.ThisFighter.Name;
				emailType = EmailManager.EmailTypes.SparRequestCancelledTemplate;
			}

			if (model.SparRequesStatus != SparRequestStatus.Canceled)
				emailPlaceholoders["[SPAR_CONFIRMATION_URL]"] = String.Format(@"<a href=""{0}\"">{0}</a>", Url.Action("SparDetailsConfirmation", "Spar", new System.Web.Routing.RouteValueDictionary() { { "ID", model.SparRequestId } }, "http", Request.Url.Host));

			SparWeb.EmailManager.SendEmail(emailType, ConfigurationManager.AppSettings["EmailSupport"], emailTo, emailSubject, emailPlaceholoders);

		}

		private ConfirmSparDetailsViewModel getConfirmSparDetailsViewModel(string sparRequestId)
		{
			SparRepository sparRepo = new SparRepository();
			SparRequest sparRequest = sparRepo.GetSparRequestById(sparRequestId);

			return Util.GetConfirmSparDetailsViewModel(sparRequest, 250, User.Identity.GetUserId());
		}

		private SparConfirmationViewModel getSparConfirmationViewModelForOpponent(string opponentId)
		{
			FighterRepository fighterRepo = new FighterRepository();

			Fighter thisFighter = fighterRepo.GetFighterByIdentityUserId(User.Identity.GetUserId());
			Fighter opponentFighter = fighterRepo.GetFighterByIdentityUserId(opponentId);

			return Util.GetSparConfirmationViewModel(thisFighter, opponentFighter, 250);
		}
	}
}