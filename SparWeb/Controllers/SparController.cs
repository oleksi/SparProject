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
			ConfirmSparDetailsViewModel confirmSparDetailsViewModel = getConfirmSparDetailsViewModel(ID);

			return View(confirmSparDetailsViewModel);
		}
		
		[Authorize]
		[HttpPost]
		public ActionResult ConfirmSparDetails(ConfirmSparDetailsViewModel model)
		{
			ConfirmSparDetailsViewModel confirmSparDetailsViewModel = getConfirmSparDetailsViewModel(model.SparRequestId);
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

				return View(confirmSparDetailsViewModel);
			}

			//updating spar request
			SparRepository sparRepo = new SparRepository();
			SparRequest sparRequest = sparRepo.GetSparRequestById(model.SparRequestId);
			sparRequest.SparDateTime = DateTime.Parse(String.Format("{0} {1}", model.SparDate.Value.ToString("MM/dd/yyyy"), model.SparTime.ToString()));
			if (model.SparGymID > 0)
				sparRequest.SparGym = confirmSparDetailsViewModel.SparGym;
			sparRequest.SparNotes = model.SparNotes;
			sparRequest.LastNegotiatorFighter = (sparRequest.RequestorFighter.SparIdentityUser.Id == User.Identity.GetUserId()) ? sparRequest.RequestorFighter : sparRequest.OpponentFighter;
			
			sparRepo.SaveSparRequest(sparRequest);

			//ToDo: send email to spar requestor

			return View("SparDetailsConfirmed", confirmSparDetailsViewModel);
		}

		private ConfirmSparDetailsViewModel getConfirmSparDetailsViewModel(string sparRequestId)
		{
			SparRepository sparRepo = new SparRepository();
			SparRequest sparRequest = sparRepo.GetSparRequestById(sparRequestId);

			//figuring out who is who
			Fighter thisFighter = null;
			Fighter opponentFighter = null;
			if (sparRequest.RequestorFighter.SparIdentityUser.Id == User.Identity.GetUserId())
			{
				thisFighter = sparRequest.RequestorFighter;
				opponentFighter = sparRequest.OpponentFighter;
			}
			else
			{
				thisFighter = sparRequest.OpponentFighter;
				opponentFighter = sparRequest.RequestorFighter; ;
			}

			AccountViewModel thisFighterAccountViewModel = Util.GetAccountViewModelForFighter(thisFighter, 250);
			AccountViewModel opponentFighterAccountViewModel = Util.GetAccountViewModelForFighter(opponentFighter, 250);

			ConfirmSparDetailsViewModel confirmSparDetailsViewModel = new ConfirmSparDetailsViewModel(getSparConfirmationViewModel(thisFighterAccountViewModel, opponentFighterAccountViewModel, 250));

			confirmSparDetailsViewModel.SparRequestId = sparRequestId;

			if (sparRequest.SparDateTime.HasValue == true)
			{
				confirmSparDetailsViewModel.SparDate = sparRequest.SparDateTime.Value.Date;
				confirmSparDetailsViewModel.SparTime = new SparTime(sparRequest.SparDateTime.Value);
			}

			if (sparRequest.SparGym != null)
			{
				confirmSparDetailsViewModel.SparGymID = sparRequest.SparGym.Id.Value;
				confirmSparDetailsViewModel.SparGym = sparRequest.SparGym;
			}

			confirmSparDetailsViewModel.SparNotes = sparRequest.SparNotes;

			return confirmSparDetailsViewModel;
		}

		private SparConfirmationViewModel getSparConfirmationViewModelForOpponent(string opponentId)
		{
			FighterRepository fighterRepo = new FighterRepository();

			Fighter thisFighter = fighterRepo.GetFighterByIdentityUserId(User.Identity.GetUserId());
			AccountViewModel thisFighterAccountViewModel = Util.GetAccountViewModelForFighter(thisFighter, 250);

			Fighter opponentFighter = fighterRepo.GetFighterByIdentityUserId(opponentId);
			AccountViewModel opponentFighterAccountViewModel = Util.GetAccountViewModelForFighter(opponentFighter, 250);

			return getSparConfirmationViewModel(thisFighterAccountViewModel, opponentFighterAccountViewModel, 250);
		}

		private SparConfirmationViewModel getSparConfirmationViewModel(AccountViewModel thisFighterAccountViewModel, AccountViewModel opponentFighterAccountViewModel, int profilePictureSize)
		{
			SparConfirmationViewModel sparConfirmationViewModel = new SparConfirmationViewModel()
			{
				ThisFighterID = thisFighterAccountViewModel.ID,
				ThisFighterName = thisFighterAccountViewModel.Name,
				ThisFighterGymName = thisFighterAccountViewModel.GymName,
				ThisFighterGym = thisFighterAccountViewModel.Gym,
				ThisProfilePictureFile = thisFighterAccountViewModel.ProfilePictureFile,
				ThisProfilePictureUploaded = thisFighterAccountViewModel.ProfilePictureUploaded,
				OpponentFighterID = opponentFighterAccountViewModel.ID,
				OpponentFighterName = opponentFighterAccountViewModel.Name,
				OpponentFighterGymName = opponentFighterAccountViewModel.GymName,
				OpponentFighterGym = opponentFighterAccountViewModel.Gym,
				OpponentProfilePictureFile = opponentFighterAccountViewModel.ProfilePictureFile,
				OpponentProfilePictureUploaded = opponentFighterAccountViewModel.ProfilePictureUploaded,
				ProfilePictureSize = profilePictureSize
			};
			return sparConfirmationViewModel;
		}
	}
}