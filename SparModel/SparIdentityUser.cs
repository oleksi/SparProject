using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections;
using System.Security.Claims;
using Microsoft.AspNet.Identity;

namespace SparModel
{
	public class SparIdentityUser : IdentityUser
	{
		public virtual async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<SparIdentityUser> manager)
		{
			// Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
			var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
			// Add custom user claims here
			return userIdentity;
		}
	}
}
