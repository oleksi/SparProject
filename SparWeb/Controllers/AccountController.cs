using System;
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
			var result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, shouldLockout: false);
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

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
			ViewBag.HeightToCentimetersMap = SparWeb.Models.Util.HeightToCentimetersMap;
			ViewBag.WeightClassMap = SparWeb.Models.Util.WeightClassMap;

			//#OS# geting a list of all gyms
			ViewBag.AllGyms = getAllGyms();

			return View(new RegisterViewModel() { Sex = true });
        }

		private Dictionary<int, string> getAllGyms()
		{
			GymRepository gymRepo = new GymRepository();
			Dictionary<int, string> allGyms = new Dictionary<int, string>();
			foreach (Gym gym in gymRepo.GetAllGyms())
				allGyms[gym.Id.Value] = gym.Name;

			return allGyms;
		}

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
				DateTime dob = DateTime.MinValue;
				if (DateTime.TryParse(String.Format("{0}/{1}/{2}", model.DateOfBirth.Month, model.DateOfBirth.Day, model.DateOfBirth.Year), out dob) == false)
				{
					ModelState.AddModelError("DateOfBirth", "Date of birth is not valid");
					return View(model);
				}

                var user = new SparIdentityUser() { UserName = model.UserName, Email = model.UserName };                
				var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {

					Gym gym = null;
					if (model.GymId.HasValue == true)
						gym = new Gym() { Id = model.GymId.Value };

					FighterRepository fighterRepo = new FighterRepository();
					Fighter fighter = new Fighter() { Name = model.Name, Sex = model.Sex, DateOfBirth = dob, Height = model.Height, Weight = model.Weight, NumberOfFights = model.NumberOfFights.Value, Gym = gym, ProfilePictureUploaded = false };
					fighter.SparIdentityUser = user;
					fighterRepo.SaveFighter(fighter);

					var code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
					var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
					await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking this link: <a href=\"" + callbackUrl + "\">link</a>");
					ViewBag.Link = callbackUrl;
					return View("DisplayEmail");
                }
                else
                {
                    AddErrors(result);
                }
            }

            // If we got this far, something failed, redisplay form
			ViewBag.HeightToCentimetersMap = SparWeb.Models.Util.HeightToCentimetersMap;
			ViewBag.WeightClassMap = SparWeb.Models.Util.WeightClassMap;
			ViewBag.AllGyms = getAllGyms();

            return View(model);
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
			return View(result.Succeeded ? "ConfirmEmail" : "Error");
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
			Fighter fighter = getLoggedInFighter();

			AccountViewModel model = null;
			if (fighter != null)
				model = new AccountViewModel() { Name = fighter.Name, GymName = fighter.Gym.Name, DateOfBirth = fighter.DateOfBirth.ToShortDateString(), Height = Util.HeightToCentimetersMap[fighter.Height], Weight = fighter.Weight, NumberOfFights = fighter.NumberOfFights };

			if (fighter.ProfilePictureUploaded == true)
				model.ProfilePictureFile = String.Format("{0}.jpg", fighter.SparIdentityUser.Id);

			return View("Account", model);
		}

		private Fighter getLoggedInFighter()
		{
			FighterRepository fighterRepo = new FighterRepository();
			Fighter fighter = fighterRepo.GetFighterByIdentityUserId(User.Identity.GetUserId());
			return fighter;
		}

		[HttpPost]
		public ActionResult UploadProfilePicture(HttpPostedFileBase file)
		{
			bool fileSavedSuccessfully = true;
			string fileName = "";
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

				Fighter fighter = getLoggedInFighter();
				fileName = String.Format("{0}.jpg", fighter.SparIdentityUser.Id);

				CloudBlockBlob blockBlob = container.GetBlockBlobReference(String.Format("ProfilePics/{0}", fileName));

				//converting to JPG
				Bitmap bitmap = new Bitmap(file.InputStream);
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

				fighter.ProfilePictureUploaded = true;
				FighterRepository fighterRepo = new FighterRepository();
				fighterRepo.SaveFighter(fighter);
			}
			catch
			{
				fileSavedSuccessfully = false;
			}

			if (fileSavedSuccessfully)
			{
				return Json(new { Message = fileName });
			}
			else
			{
				return Json(new { Message = "Error" });
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

				var code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
				var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
				await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking here: <a href=\"" + callbackUrl + "\">link</a>");
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
			return RedirectToAction("Index", "Home");
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