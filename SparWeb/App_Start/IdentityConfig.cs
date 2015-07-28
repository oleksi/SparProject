using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using SparData;
using SparModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;

namespace SparWeb.Models
{
	public class ApplicationUserManager : UserManager<SparIdentityUser>
	{
		public ApplicationUserManager(IUserStore<SparIdentityUser> store)
			: base(store)
		{
		}

		public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options,
			IOwinContext context)
		{
			//var manager = new ApplicationUserManager(new UserStore<SparIdentityUser>(context.Get<ApplicationDbContext>()));
			var manager = new ApplicationUserManager(new SparIdentityUserStore<SparIdentityUser>());
			// Configure validation logic for usernames
			manager.UserValidator = new UserValidator<SparIdentityUser>(manager)
			{
				AllowOnlyAlphanumericUserNames = false,
				RequireUniqueEmail = true
			};
			// Configure validation logic for passwords
			manager.PasswordValidator = new PasswordValidator
			{
				RequiredLength = 6,
				RequireNonLetterOrDigit = false,
				RequireDigit = false,
				RequireLowercase = false,
				RequireUppercase = false,
			};
			// Configure user lockout defaults
			manager.UserLockoutEnabledByDefault = true;
			manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
			manager.MaxFailedAccessAttemptsBeforeLockout = 5;
			// Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
			// You can write your own provider and plug in here.
			manager.RegisterTwoFactorProvider("PhoneCode", new PhoneNumberTokenProvider<SparIdentityUser>
			{
				MessageFormat = "Your security code is: {0}"
			});
			manager.RegisterTwoFactorProvider("EmailCode", new EmailTokenProvider<SparIdentityUser>
			{
				Subject = "SecurityCode",
				BodyFormat = "Your security code is {0}"
			});
			manager.EmailService = new EmailService();
			manager.SmsService = new SmsService();
			var dataProtectionProvider = options.DataProtectionProvider;
			if (dataProtectionProvider != null)
			{
				manager.UserTokenProvider =
					new DataProtectorTokenProvider<SparIdentityUser>(dataProtectionProvider.Create("ASP.NET Identity"));
			}
			return manager;
		}
	}

	// Configure the RoleManager used in the application. RoleManager is defined in the ASP.NET Identity core assembly
	public class ApplicationRoleManager : RoleManager<SparIdentityRole>
	{
		public ApplicationRoleManager(IRoleStore<SparIdentityRole, string> roleStore)
			: base(roleStore)
		{
		}

		public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context)
		{
			return new ApplicationRoleManager(new SparIdentityRoleStore<SparIdentityRole>());
			//return new ApplicationRoleManager(new RoleStore<IdentityRole>(context.Get<ApplicationDbContext>()));
		}
	}

	public class EmailService : IIdentityMessageService
	{
		public Task SendAsync(IdentityMessage message)
		{
			SparWeb.EmailManager.SendEmail(ConfigurationManager.AppSettings["EmailSupport"], message.Destination, message.Subject, message.Body);

			// Plug in your email service here to send an email.
			return Task.FromResult(0);
		}
	}

	public class SmsService : IIdentityMessageService
	{
		public Task SendAsync(IdentityMessage message)
		{
			// Plug in your sms service here to send a text message.
			return Task.FromResult(0);
		}
	}

	public class ApplicationSignInManager : SignInManager<SparIdentityUser, string>
	{
		public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager) :
			base(userManager, authenticationManager) { }

		public override Task<ClaimsIdentity> CreateUserIdentityAsync(SparIdentityUser user)
		{
			return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
		}

		public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
		{
			return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
		}
	}
}