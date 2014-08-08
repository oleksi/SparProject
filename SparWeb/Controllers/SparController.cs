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
			FighterRepository fighterRepo = new FighterRepository();
			
			Fighter thisFighter = fighterRepo.GetFighterByIdentityUserId(User.Identity.GetUserId());
			AccountViewModel thisFighterAccountViewModel = Util.GetAccountViewModelForFighter(thisFighter, 250);

			Fighter opponentFighter = fighterRepo.GetFighterByIdentityUserId(ID);
			AccountViewModel opponentFighterAccountViewModel = Util.GetAccountViewModelForFighter(opponentFighter, 250);

			SparConfirmationViewModel sparConfirmationViewModel = getSparConfirmationViewModel(thisFighterAccountViewModel, opponentFighterAccountViewModel);

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
				String emailSubject = String.Format("{0} wants to spar you!", opponentFighter.Name);
				String emailBody = String.Format("{0} wants to spar you. If you are interested, please click the link below to select date and place of spar:<br /><br /><a href=\"{1}\">{1}</a>", 
					opponentFighter.Name,
					Url.Action("ConfirmSparDetails", "Spar", new System.Web.Routing.RouteValueDictionary() { { "SparRequestId", sparRequest.Id } }, "http", Request.Url.Host)
				);

				SparWeb.Util.SendEmail(emailTo, emailSubject, emailBody);
			}
			catch {}

			return View("ConfirmSpar", (Object)opponentFighter.Name);
		}

		[Authorize]
		public ActionResult ConfirmSparDetails(string sparRequestId)
		{
			SparRepository sparRepo = new SparRepository();
			SparRequest sparRequest = sparRepo.GetSparRequestById(sparRequestId);

			AccountViewModel thisFighterAccountViewModel = Util.GetAccountViewModelForFighter(sparRequest.RequestorFighter, 250);
			AccountViewModel opponentFighterAccountViewModel = Util.GetAccountViewModelForFighter(sparRequest.OpponentFighter, 250);

			ConfirmSparDetailsViewModel confirmSparDetailsViewModel = new ConfirmSparDetailsViewModel(getSparConfirmationViewModel(thisFighterAccountViewModel, opponentFighterAccountViewModel));
			confirmSparDetailsViewModel.SparRequestId = sparRequestId;

			return View(confirmSparDetailsViewModel);
		}

		private SparConfirmationViewModel getSparConfirmationViewModel(AccountViewModel thisFighterAccountViewModel, AccountViewModel opponentFighterAccountViewModel)
		{
			SparConfirmationViewModel sparConfirmationViewModel = new SparConfirmationViewModel()
			{
				ThisFighterID = thisFighterAccountViewModel.ID,
				ThisFighterName = thisFighterAccountViewModel.Name,
				ThisFighterGymName = thisFighterAccountViewModel.GymName,
				ThisProfilePictureFile = thisFighterAccountViewModel.ProfilePictureFile,
				ThisProfilePictureUploaded = thisFighterAccountViewModel.ProfilePictureUploaded,
				OpponentFighterID = opponentFighterAccountViewModel.ID,
				OpponentFighterName = opponentFighterAccountViewModel.Name,
				OpponentFighterGymName = opponentFighterAccountViewModel.GymName,
				OpponentProfilePictureFile = opponentFighterAccountViewModel.ProfilePictureFile,
				OpponentProfilePictureUploaded = opponentFighterAccountViewModel.ProfilePictureUploaded
			};
			return sparConfirmationViewModel;
		}
	}
}