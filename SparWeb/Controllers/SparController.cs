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
				Status = SparRequestStatus.Requested
			};

			//saving spar request
			SparRepository sparRepo = new SparRepository();
			sparRepo.CreateSparRequest(sparRequest);

			//sending out notification email
			try
			{
				String emailTo = opponentFighter.SparIdentityUser.Email;
				String emailSubject = String.Format("{0} wants to spar you!", thisFighter.Name);
				String emailBody = String.Format("{0} wants to spar you. If you are interested, please click the link below to select date and place of spar:<br /><br /><a href=\"{1}\">{1}</a>",
					thisFighter.Name,
					Url.Action("SparDetailsConfirmation", "Spar", new System.Web.Routing.RouteValueDictionary() { { "ID", sparRequest.Id } }, "http", Request.Url.Host)
				);

				SparWeb.Util.SendEmail(emailTo, emailSubject, emailBody);
			}
			catch {}

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
			sparRequest.LastNegotiatorFighter = model.ThisFighter;
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
			try
			{
				string emailTo = model.OpponentFighter.SparIdentityUser.Email;

				string emailSubject = "";
				string emailBody = "";
				string emailBodySparDetails = String.Format(@"
<br /><br />
Date: {0}<br />
Time: {1}<br />
Location: {2}<br />
{3} 
<br /><br />
",
					model.SparDate.Value.ToString("MM/dd/yyyy"),
					model.SparTime.ToString(),
					(model.SparGymID > 0) ? model.SparGym.Name : "N/A",
					(String.IsNullOrEmpty(model.SparNotes) == false) ? String.Format("Notes: {0}<br />", model.SparNotes) : ""
				);

				if (isFirstResponse == true) // first response
				{
					emailSubject = String.Format("{0} has confirmed your spar request", model.ThisFighter.Name);
					emailBody = String.Format(@"{0} has confirmed your spar request. {1} proposes the following date, time and location for the spar: ", model.ThisFighter.Name, model.ThisFighter.GetHeOrShe(true));
					emailBody += emailBodySparDetails;
					emailBody += @"
Please click the link below to confirm spar or change the details:
<br /><br />
";
				}
				else if (model.SparRequesStatus == SparRequestStatus.DateLocationNegotiation) // spar negotiation
				{
					emailSubject = String.Format("{0} updated spar details", model.ThisFighter.Name);
					emailBody = String.Format("{0} proposes the following date, time and location for the spar: ", model.ThisFighter.Name);
					emailBody += emailBodySparDetails;
					emailBody += @"
Please click the link below to confirm spar or change the details:
<br /><br />
";
				}
				else if (model.SparRequesStatus == SparRequestStatus.Confirmed) // spar is confirmed
				{
					emailSubject = String.Format("{0} has confirmed the spar", model.ThisFighter.Name);
					emailBody = String.Format(@"{0} has confirmed the spar. Here are date, time and location for the spar: ", model.ThisFighter.Name);
					emailBody += emailBodySparDetails;
					emailBody += @"
If you ever want to cancel the spar, please use the link below:
<br /><br />
";
				}
				else if (model.SparRequesStatus == SparRequestStatus.Canceled) // spar is canceled
				{
					emailSubject = String.Format("{0} has canceled the spar", model.ThisFighter.Name);
					emailBody = String.Format(@"{0} has canceled the following spar: ", model.ThisFighter.Name);
					emailBody += emailBodySparDetails;
					emailBody += @"
<br /><br />
";
				}

				if (model.SparRequesStatus != SparRequestStatus.Canceled)
					emailBody += String.Format(@"<a href=""{0}\"">{0}</a>", Url.Action("SparDetailsConfirmation", "Spar", new System.Web.Routing.RouteValueDictionary() { { "ID", model.SparRequestId } }, "http", Request.Url.Host));

				SparWeb.Util.SendEmail(emailTo, emailSubject, emailBody);
			}
			catch { }
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