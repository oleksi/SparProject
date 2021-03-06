﻿using SparData;
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
			var currIdentityUserId = User.Identity.GetUserId();

			var fighterRepo = new FighterRepository();
			var fighter = fighterRepo.GetFighterByIdentityUserId(currIdentityUserId);

			if (fighter != null)
				return SparConfirmationComplete(currIdentityUserId, ID);

			//if we got here, we're dealing with trainer

			var trainerRepo = new TrainerRepository();
			var trainer = trainerRepo.GetTrainerByIdentityUserId(currIdentityUserId);

			//getting trainer's fighters
			var fightersList = fighterRepo.GetAllFighters().Where(ff => ff.Trainer != null && ff.Trainer.Id == trainer.Id).ToList();
			var fightersAccountViewModelList = new List<AccountFighterViewModel>();
			foreach (var currFighter in fightersList)
			{
				var fighterAccountViewModel = Util.GetAccountViewModelForFighter(currFighter, 150);
				fighterAccountViewModel.IsFighterSelectView = true;

				fightersAccountViewModelList.Add(fighterAccountViewModel);
			}
			var opponentFighter = fighterRepo.GetFighterByIdentityUserId(ID);

			var selectFighterViewModel = new SelectFighterViewModel();
			selectFighterViewModel.FightersList = fightersAccountViewModelList;
			selectFighterViewModel.OpponentFighterId = ID;
			selectFighterViewModel.OpponentFighterName = opponentFighter.Name;

			return View("SelectFighter", selectFighterViewModel);
		}

		[Authorize]
		[HttpPost]
		public ActionResult SparConfirmation(SelectFighterViewModel model)
		{
			return SparConfirmationComplete(model.SelectedFighterId, model.OpponentFighterId);
		}

		[Authorize]
		public ActionResult SparConfirmationComplete(string fighterId, string opponentId)
		{
			FighterRepository fighterRepo = new FighterRepository();

			int thisFighterId = fighterRepo.GetFighterByIdentityUserId(fighterId).Id.Value;
			int opponentFighterId = fighterRepo.GetFighterByIdentityUserId(opponentId).Id.Value;

			//checking if current user has already requested, negotiated or confirmed the spar with this fighter and if so not letting to request another one
			bool alreadyRequestedSpar = Util.GetSparRequestDetailsForFighter(opponentFighterId, fighterId).Any(sr => (sr.ThisFighter.Id == thisFighterId || sr.OpponentFighter.Id == thisFighterId));
			if (alreadyRequestedSpar == true)
				return RedirectToAction("Index", "Home");

			SparConfirmationViewModel sparConfirmationViewModel = getSparConfirmationViewModel(fighterId, opponentId);

			return View(sparConfirmationViewModel);
		}

		[Authorize]
		[HttpPost]
		public ActionResult ConfirmSpar(string FighterId, string OpponentId, string SparNotes)
		{
			var sparConfViewModel = getSparConfirmationViewModel(FighterId, OpponentId);
			if (ModelState.IsValid == false)
			{
				return View("SparConfirmation", sparConfViewModel);
			}

			FighterRepository fighterRepo = new FighterRepository();
			Fighter thisFighter = fighterRepo.GetFighterByIdentityUserId(FighterId);
			Fighter opponentFighter = fighterRepo.GetFighterByIdentityUserId(OpponentId);

			SparRequest sparRequest = new SparRequest()
			{
				OpponentFighter = opponentFighter,
				RequestDate = DateTime.Now,
				RequestorFighter = thisFighter,
				Status = SparRequestStatus.Requested,
				LastNegotiatorFighterId = thisFighter.Id.Value,
				SparNotes = SparNotes
			};

			//saving spar request
			SparRepository sparRepo = new SparRepository();
			sparRepo.CreateSparRequest(sparRequest);

			//sending spar request email
			string emailTo = "";
			string emailSubject = String.Format("New Spar Request");
			Dictionary<string, string> emailPlaceholders = new Dictionary<string, string>();

			var prifilePicUrl = Util.GetProfilePictureFile(thisFighter, 150);
			if (prifilePicUrl.StartsWith("http://") == false && prifilePicUrl.StartsWith("https://") == false)
				prifilePicUrl = "http://fightura.com" + prifilePicUrl;
			emailPlaceholders["[FIGTHER_PROFILE_PICTURE_URL]"] = prifilePicUrl;

			emailPlaceholders["[FIGTHER_NAME]"] = "<a href=\"http://fightura.com/" + sparConfViewModel.ThisFighterUrl + "\">" + thisFighter.Name + "</a>";
			emailPlaceholders["[HIS_OR_HER]"] = (thisFighter.Sex == true) ? "his" : "her";
			emailPlaceholders["[AGE]"] = thisFighter.GetMemberAge().ToString();
			emailPlaceholders["[CITY]"] = thisFighter.City;
			emailPlaceholders["[STATE]"] = thisFighter.State;
			emailPlaceholders["[HEIGHT]"] = Util.HeightToCentimetersMap[thisFighter.Height];
			emailPlaceholders["[WEIGHT]"] = Util.WeightClassMap[thisFighter.Weight];
			emailPlaceholders["[STANCE]"] = (thisFighter.IsSouthpaw) ? "Left-handed" : "Right-handed";
			emailPlaceholders["[NUMBER_OF_AMATEUR_FIGHTS]"] = thisFighter.NumberOfAmateurFights.ToString();
			emailPlaceholders["[NUMBER_OF_PRO_FUIGTS]"] = (thisFighter.NumberOfProFights > 0) ? " / " + thisFighter.NumberOfAmateurFights + " pro" : "";
			emailPlaceholders["[GYM]"] = (thisFighter.Gym == null) ? "" : "Gym: " + thisFighter.Gym.Name + "<br />";
			emailPlaceholders["[SPAR_NOTES]"] = (String.IsNullOrEmpty(SparNotes) == false) ? "<br /><b>Message:</b> " + SparNotes : "";
			emailPlaceholders["[SPAR_CONFIRMATION_URL]"] = Url.Action("SparDetailsConfirmation", "Spar", new System.Web.Routing.RouteValueDictionary() { { "ID", sparRequest.Id } }, "http", Request.Url.Host);
			//figuring out if email should be sent to fighter or fighter's trainer
			if (opponentFighter.Trainer != null)
			{
				emailTo = opponentFighter.Trainer.SparIdentityUser.Email;
				emailPlaceholders["[NAME]"] = opponentFighter.Trainer.Name;
			}
			else
			{
				emailTo = opponentFighter.SparIdentityUser.Email;
				emailPlaceholders["[NAME]"] = opponentFighter.Name;
			}

			//sending out spar request email
			EmailManager.EmailTypes emailType = 0;
			if (thisFighter.Trainer != null && opponentFighter.Trainer != null)
			{
				//trainer to trainer
				emailPlaceholders["[TRAINER_NAME]"] = thisFighter.Trainer.Name;
				emailPlaceholders["[TRAINER_LOCATION]"] = String.Format("{0}, {1}", thisFighter.Trainer.City, thisFighter.Trainer.State);
				emailPlaceholders["[OPPONENT_TRAINER_FIGHTER_NAME]"] = opponentFighter.Name;
				emailType = EmailManager.EmailTypes.SparRequestInitialTrainerToTrainerTemplate;
			}
			else if (thisFighter.Trainer == null && opponentFighter.Trainer != null)
			{
				//fighter to trainer
				emailPlaceholders["[OPPONENT_TRAINER_FIGHTER_NAME]"] = opponentFighter.Name;
				emailType = EmailManager.EmailTypes.SparRequestInitialFighterToTrainerTemplate;
			}
			else if (thisFighter.Trainer != null && opponentFighter.Trainer == null)
			{
				//trainer to fighter
				emailPlaceholders["[TRAINER_NAME]"] = thisFighter.Trainer.Name;
				emailPlaceholders["[TRAINER_LOCATION]"] = String.Format("{0}, {1}", thisFighter.Trainer.City, thisFighter.Trainer.State);
				emailType = EmailManager.EmailTypes.SparRequestInitialTrainerToFighterTemplate;
			}
			else
			{
				//fighter to fighter
				emailType = EmailManager.EmailTypes.SparRequestInitialFighterToFighterTemplate;
			}
			SparWeb.EmailManager.SendEmail(emailType, ConfigurationManager.AppSettings["EmailSupport"], emailTo, emailSubject, emailPlaceholders);

			//sending out notification email
			emailPlaceholders = new Dictionary<string, string>();
			emailPlaceholders["[NAME]"] = ConfigurationManager.AppSettings["AdminName"];
			emailPlaceholders["[REQUESTOR_FIGHTER_NAME]"] = thisFighter.Name;
			emailPlaceholders["[OPPONENT_FIGHTER_NAME]"] = opponentFighter.Name;
			EmailManager.SendEmail(EmailManager.EmailTypes.SparRequestNotificationTemplate, ConfigurationManager.AppSettings["EmailSupport"], ConfigurationManager.AppSettings["EmailAdmin"], "New Spar Rquest Was Initiated!", emailPlaceholders);

			SparConfirmationViewModel sparConfirmationViewModel = getSparConfirmationViewModel(FighterId, OpponentId);
			return View("SparConfirmed", sparConfirmationViewModel);
		}

		[Authorize]
		public ActionResult SparDetailsConfirmation(string ID, bool? ShowSuccess)
		{
			SparRepository sparRepo = new SparRepository();
			SparRequest sparRequest = sparRepo.GetSparRequestById(ID);

			ConfirmSparDetailsViewModel confirmSparDetailsViewModel = Util.GetConfirmSparDetailsViewModel(sparRequest, 250, User.Identity.GetUserId()); 

			//checking if this user is authorized to view this page
			var relatedFightersList = Util.GetRelatedFightersList(User.Identity.GetUserId());
			if (relatedFightersList.Contains(confirmSparDetailsViewModel.OpponentFighter.Id.Value) == false && relatedFightersList.Contains(confirmSparDetailsViewModel.ThisFighter.Id.Value) == false)
				return RedirectToAction("Index", "Home");

			List<SparNoteViewModel> sparNotesVM = getSparNotesForSparRequest(ID);
			if (sparNotesVM.Count == 0 && String.IsNullOrEmpty(sparRequest.SparNotes) == false)
			{
				var figherRepo = new FighterRepository();
				var fighter = figherRepo.GetFighterById(sparRequest.LastNegotiatorFighterId);
				var sparNoteVM = new SparNoteViewModel()
				{
					Name = fighter.Name,
					ProfilePictureFile = Util.GetProfilePictureFile(fighter, 150),
					NoteDate = sparRequest.RequestDate,
					SparNotes = sparRequest.SparNotes
				};
				sparNotesVM.Add(sparNoteVM);
			}

			confirmSparDetailsViewModel.SparNotesList = sparNotesVM;
			confirmSparDetailsViewModel.ListSparNotes = true;

			if (ShowSuccess.HasValue == true && ShowSuccess == true)
				ViewBag.ShowSuccessMessage = true;

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
			confirmSparDetailsViewModel.ListSparNotes = true;
			
			if (model.SparGymID.HasValue && model.SparGymID.Value > 0)
			{
				GymRepository gymRepo = new GymRepository();
				confirmSparDetailsViewModel.SparGym = gymRepo.GetGymById(model.SparGymID.Value);
				confirmSparDetailsViewModel.SparGymID = model.SparGymID;
			}
			else
			{
				confirmSparDetailsViewModel.SparGym = null;
			}

			confirmSparDetailsViewModel.SparNotes = model.SparNotesNew;

			if (ModelState.IsValid == false) //|| model.SparDate <= DateTime.Now)
			{
				//if (model.SparDate <= DateTime.Now)
				//	ModelState.AddModelError("SparDate", "Spar Date must be in the future");

				confirmSparDetailsViewModel.SparNotesList = getSparNotesForSparRequest(model.SparRequestId);

				return View("SparDetailsConfirmation", confirmSparDetailsViewModel);
			}

			confirmSparDetailsViewModel.SparRequesStatus = model.SparRequesStatus;

			//checking if it's 1st response to the spar request
			bool isFirstResponse = (sparRequest.Status == SparRequestStatus.Requested);

			//updating spar request
			if (model.SparRequesStatus == SparRequestStatus.DateLocationNegotiation)
			{
				if (model.SparDate.HasValue)
				{
					if (model.SparTime.Hours.HasValue == false)
					{
						model.SparTime.Hours = 12;
						model.SparTime.Minutes = 0;
						model.SparTime.IsAM = false;
					}
					sparRequest.SparDateTime = DateTime.Parse(String.Format("{0} {1}", model.SparDate.Value.ToString("MM/dd/yyyy"), model.SparTime.ToString()));
				}
				if (model.SparGymID.HasValue && model.SparGymID.Value > 0)
					sparRequest.SparGym = confirmSparDetailsViewModel.SparGym;
				else
					sparRequest.SparGym = null;

				sparRequest.SparNotes = model.SparNotesNew;
			}

			//setting last negotiator
			var fighterRepo = new FighterRepository();
			var fighter = fighterRepo.GetFighterByIdentityUserId(User.Identity.GetUserId());
			if (fighter != null)
			{
				//if it's fighter then he is the last negotiator
				sparRequest.LastNegotiatorFighterId = fighter.Id.Value;
			}
			else 
			{
				//if it's trainer than one of the spar request fighters is his fighter
				var trainerRepo = new TrainerRepository();
				var trainer = trainerRepo.GetTrainerByIdentityUserId(User.Identity.GetUserId());
				if (sparRequest.RequestorFighter.Trainer != null && sparRequest.RequestorFighter.Trainer.Id == trainer.Id)
					sparRequest.LastNegotiatorFighterId = sparRequest.RequestorFighter.Id.Value;
				else
					sparRequest.LastNegotiatorFighterId = sparRequest.OpponentFighter.Id.Value;
			}

			sparRequest.Status = model.SparRequesStatus;

			sparRepo.SaveSparRequest(sparRequest);

			//send email to spar requestor
			sendEmailForSaprRequest(confirmSparDetailsViewModel, isFirstResponse);

			return RedirectToAction("SparDetailsConfirmation", new { ID = model.SparRequestId, ShowSuccess = true }); //View("SparDetailsConfirmed", confirmSparDetailsViewModel);
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
			string emailTo = (model.OpponentFighter.Trainer != null) ? model.OpponentFighter.Trainer.SparIdentityUser.Email : model.OpponentFighter.SparIdentityUser.Email;

			string emailSubject = "";
			SparWeb.EmailManager.EmailTypes emailType = 0;

			var emailPlaceholoders = new Dictionary<string, string>();
			emailPlaceholoders["[SPAR_DATE]"] = (model.SparDate.HasValue)? model.SparDate.Value.ToString("MM/dd/yyyy") : "N/A";
			emailPlaceholoders["[SPAR_TIME]"] = (model.SparTime != null)? model.SparTime.ToString() : "N/A";
			emailPlaceholoders["[SPAR_LOCATION]"] = (model.SparGym != null) ? model.SparGym.Name : "N/A";
			emailPlaceholoders["[SPAR_NOTES]"] = (String.IsNullOrEmpty(model.SparNotes) == false) ? String.Format("Notes: {0}<br />", model.SparNotes) : "";
				
			if (isFirstResponse == true) // first response
			{
				emailSubject = String.Format("{0} has confirmed your spar request", model.ThisFighter.Name);
				if (model.ThisFighter.Trainer != null && model.OpponentFighter.Trainer != null)
				{
					//trainer to trainer
					emailPlaceholoders["[NAME]"] = model.OpponentFighter.Trainer.Name;
					emailPlaceholoders["[TRAINER_NAME]"] = model.ThisFighter.Trainer.Name;
					emailPlaceholoders["[FIGTHER_NAME]"] = model.ThisFighter.Name;
					emailType = EmailManager.EmailTypes.SparRequestFirstTimeResponseTrainerToTrainerTemplate;
				}
				else if (model.ThisFighter.Trainer == null && model.OpponentFighter.Trainer != null)
				{
					//fighter to trainer
					emailPlaceholoders["[NAME]"] = model.OpponentFighter.Trainer.Name;
					emailPlaceholoders["[FIGTHER_NAME]"] = model.ThisFighter.Name;
					emailPlaceholoders["[HE_OR_SHE]"] = model.ThisFighter.GetHeOrShe(true);
					emailType = EmailManager.EmailTypes.SparRequestFirstTimeResponseFighterToTrainerTemplate;
				}
				else if (model.ThisFighter.Trainer != null && model.OpponentFighter.Trainer == null)
				{
					//trainer to fighter
					emailPlaceholoders["[NAME]"] = model.OpponentFighter.Name;
					emailPlaceholoders["[TRAINER_NAME]"] = model.ThisFighter.Trainer.Name;
					emailPlaceholoders["[FIGTHER_NAME]"] = model.ThisFighter.Name;
					emailPlaceholoders["[HE_OR_SHE]"] = model.ThisFighter.GetHeOrShe(true);
					emailType = EmailManager.EmailTypes.SparRequestFirstTimeResponseTrainerToFighterTemplate;
				}
				else
				{
					//fighter to fighter
					emailPlaceholoders["[NAME]"] = model.OpponentFighter.Name;
					emailPlaceholoders["[FIGTHER_NAME]"] = model.ThisFighter.Name;
					emailPlaceholoders["[HE_OR_SHE]"] = model.ThisFighter.GetHeOrShe(true);
					emailType = EmailManager.EmailTypes.SparRequestFirstTimeResponseFighterToFighterTemplate;
				}
			}
			else if (model.SparRequesStatus == SparRequestStatus.DateLocationNegotiation) // spar negotiation
			{
				if (model.ThisFighter.Trainer != null && model.OpponentFighter.Trainer != null)
				{
					//trainer to trainer
					emailPlaceholoders["[NAME]"] = model.OpponentFighter.Trainer.Name;
					emailSubject = String.Format("{0} updated spar details", model.ThisFighter.Trainer.Name);
					emailPlaceholoders["[TRAINER_NAME]"] = model.ThisFighter.Trainer.Name;
					emailPlaceholoders["[FIGTHER_NAME]"] = model.ThisFighter.Name;
					emailType = EmailManager.EmailTypes.SparRequestNegotiationTrainerToTrainerTemplate;
				}
				else if (model.ThisFighter.Trainer == null && model.OpponentFighter.Trainer != null)
				{
					//fighter to trainer
					emailPlaceholoders["[NAME]"] = model.OpponentFighter.Trainer.Name;
					emailSubject = String.Format("{0} updated spar details", model.ThisFighter.Name);
					emailPlaceholoders["[FIGTHER_NAME]"] = model.ThisFighter.Name;
					emailType = EmailManager.EmailTypes.SparRequestNegotiationFighterToTrainerTemplate;
				}
				else if (model.ThisFighter.Trainer != null && model.OpponentFighter.Trainer == null)
				{
					//trainer to fighter
					emailPlaceholoders["[NAME]"] = model.OpponentFighter.Name;
					emailSubject = String.Format("{0} updated spar details", model.ThisFighter.Trainer.Name);
					emailPlaceholoders["[TRAINER_NAME]"] = model.ThisFighter.Trainer.Name;
					emailPlaceholoders["[FIGTHER_NAME]"] = model.ThisFighter.Name;
					emailType = EmailManager.EmailTypes.SparRequestNegotiationTrainerToFighterTemplate;
				}
				else
				{
					//fighter to fighter
					emailPlaceholoders["[NAME]"] = model.OpponentFighter.Name;
					emailSubject = String.Format("{0} updated spar details", model.ThisFighter.Name);
					emailPlaceholoders["[FIGTHER_NAME]"] = model.ThisFighter.Name;
					emailType = EmailManager.EmailTypes.SparRequestNegotiationFighterToFighterTemplate;
				}
			}
			else if (model.SparRequesStatus == SparRequestStatus.Confirmed) // spar is confirmed
			{
				if (model.ThisFighter.Trainer != null && model.OpponentFighter.Trainer != null)
				{
					//trainer to trainer
					emailPlaceholoders["[NAME]"] = model.OpponentFighter.Trainer.Name;
					emailSubject = String.Format("{0} has confirmed the spar", model.ThisFighter.Trainer.Name);
					emailPlaceholoders["[TRAINER_NAME]"] = model.ThisFighter.Trainer.Name;
					emailPlaceholoders["[FIGTHER_NAME]"] = model.ThisFighter.Name;
					emailType = EmailManager.EmailTypes.SparRequestConfirmedTrainerToTrainerTemplate;
				}
				else if (model.ThisFighter.Trainer == null && model.OpponentFighter.Trainer != null)
				{
					//fighter to trainer
					emailPlaceholoders["[NAME]"] = model.OpponentFighter.Trainer.Name;
					emailSubject = String.Format("{0} has confirmed the spar", model.ThisFighter.Name);
					emailPlaceholoders["[FIGTHER_NAME]"] = model.ThisFighter.Name;
					emailType = EmailManager.EmailTypes.SparRequestConfirmedFighterToTrainerTemplate;
				}
				else if (model.ThisFighter.Trainer != null && model.OpponentFighter.Trainer == null)
				{
					//trainer to fighter
					emailPlaceholoders["[NAME]"] = model.OpponentFighter.Name;
					emailSubject = String.Format("{0} has confirmed the spar", model.ThisFighter.Trainer.Name);
					emailPlaceholoders["[TRAINER_NAME]"] = model.ThisFighter.Trainer.Name;
					emailPlaceholoders["[FIGTHER_NAME]"] = model.ThisFighter.Name;
					emailType = EmailManager.EmailTypes.SparRequestConfirmedTrainerToFighterTemplate;
				}
				else
				{
					//fighter to fighter
					emailPlaceholoders["[NAME]"] = model.OpponentFighter.Name;
					emailSubject = String.Format("{0} has confirmed the spar", model.ThisFighter.Name);
					emailPlaceholoders["[FIGTHER_NAME]"] = model.ThisFighter.Name;
					emailType = EmailManager.EmailTypes.SparRequestConfirmedFighterToFighterTemplate;
				}
			}
			else if (model.SparRequesStatus == SparRequestStatus.Canceled) // spar is cancelled
			{
				if (model.ThisFighter.Trainer != null && model.OpponentFighter.Trainer != null)
				{
					//trainer to trainer
					emailPlaceholoders["[NAME]"] = model.OpponentFighter.Trainer.Name;
					emailSubject = String.Format("{0} has cancelled the spar", model.ThisFighter.Trainer.Name);
					emailPlaceholoders["[TRAINER_NAME]"] = model.ThisFighter.Trainer.Name;
					emailPlaceholoders["[FIGTHER_NAME]"] = model.ThisFighter.Name;
					emailType = EmailManager.EmailTypes.SparRequestCancelledTrainerToTrainerTemplate;
				}
				else if (model.ThisFighter.Trainer == null && model.OpponentFighter.Trainer != null)
				{
					//fighter to trainer
					emailPlaceholoders["[NAME]"] = model.OpponentFighter.Trainer.Name;
					emailSubject = String.Format("{0} has cancelled the spar", model.ThisFighter.Name);
					emailPlaceholoders["[FIGTHER_NAME]"] = model.ThisFighter.Name;
					emailType = EmailManager.EmailTypes.SparRequestCancelledFighterToTrainerTemplate;
				}
				else if (model.ThisFighter.Trainer != null && model.OpponentFighter.Trainer == null)
				{
					//trainer to fighter
					emailPlaceholoders["[NAME]"] = model.OpponentFighter.Name;
					emailSubject = String.Format("{0} has cancelled the spar", model.ThisFighter.Trainer.Name);
					emailPlaceholoders["[TRAINER_NAME]"] = model.ThisFighter.Trainer.Name;
					emailPlaceholoders["[FIGTHER_NAME]"] = model.ThisFighter.Name;
					emailType = EmailManager.EmailTypes.SparRequestCancelledTrainerToFighterTemplate;
				}
				else
				{
					//fighter to fighter
					emailPlaceholoders["[NAME]"] = model.OpponentFighter.Name;
					emailSubject = String.Format("{0} has cancelled the spar", model.ThisFighter.Name);
					emailPlaceholoders["[FIGTHER_NAME]"] = model.ThisFighter.Name;
					emailType = EmailManager.EmailTypes.SparRequestCancelledFighterToFighterTemplate;
				}
			}

			if (model.SparRequesStatus != SparRequestStatus.Canceled)
				emailPlaceholoders["[SPAR_CONFIRMATION_URL]"] = String.Format(@"<a href=""{0}\"">{0}</a>", Url.Action("SparDetailsConfirmation", "Spar", new System.Web.Routing.RouteValueDictionary() { { "ID", model.SparRequestId } }, "http", Request.Url.Host));

			SparWeb.EmailManager.SendEmail(emailType, ConfigurationManager.AppSettings["EmailSupport"], emailTo, emailSubject, emailPlaceholoders);

		}

		private SparConfirmationViewModel getSparConfirmationViewModel(string opponentId)
		{
			return getSparConfirmationViewModel(User.Identity.GetUserId(), opponentId);
		}

		private SparConfirmationViewModel getSparConfirmationViewModel(string fighterId, string opponentId)
		{
			FighterRepository fighterRepo = new FighterRepository();

			Fighter thisFighter = fighterRepo.GetFighterByIdentityUserId(fighterId);
			Fighter opponentFighter = fighterRepo.GetFighterByIdentityUserId(opponentId);

			return Util.GetSparConfirmationViewModel(thisFighter, opponentFighter, 250);
		}

		private List<SparNoteViewModel> getSparNotesForSparRequest(string sparRequestId)
		{
			List<SparNoteViewModel> sparNotesVM = new List<SparNoteViewModel>();
			var figherRepo = new FighterRepository();
			var sparRepo = new SparRepository();

			var sparNotes = sparRepo.GetSparNotes(sparRequestId);
			if (sparNotes != null && sparNotes.Count > 0)
			{
				foreach (var sparNote in sparNotes)
				{
					var fighter = figherRepo.GetFighterById(sparNote.MemberId);
					var sparNoteVM = new SparNoteViewModel()
					{
						Name = fighter.Name,
						ProfilePictureFile = Util.GetProfilePictureFile(fighter, 150),
						NoteDate = sparNote.NoteDate,
						SparNotes = sparNote.SparNotes
					};
					sparNotesVM.Add(sparNoteVM);
				}
			}

			return sparNotesVM;
		}
	}
}