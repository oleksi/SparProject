using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SparModel;
using Microsoft.AspNet.Identity;

namespace SparData
{
	public class SparIdentityUserStore<TUser> : IUserStore<TUser>, IUserLockoutStore<TUser, string>, IUserPasswordStore<TUser>, IUserTwoFactorStore<TUser, string>, IUserPhoneNumberStore<TUser>, IUserLoginStore<TUser>, IUserEmailStore<TUser>, IUserSecurityStampStore<TUser>, IUserClaimStore<TUser>, IUserRoleStore<TUser>, IQueryableUserStore<TUser> where TUser : SparIdentityUser
	{
		private static ISession getSession()
		{
			return SessionProvider.SessionFactory.OpenSession();
		}

		public Task CreateAsync(TUser user)
		{
			using (var session = getSession())
			{
				using (var transaction = session.BeginTransaction())
				{
					session.Save(user);
					transaction.Commit();
				}
			}

			return Task.FromResult<Object>(null);
		}

		public Task DeleteAsync(TUser user)
		{
			using (var session = getSession())
			{
				using (var transaction = session.BeginTransaction())
				{
					session.Delete(user);
					transaction.Commit();
				}
			}

			return Task.FromResult<Object>(null);
		}

		public Task<TUser> FindByIdAsync(string userId)
		{
			using (var session = getSession())
			{
				return Task.FromResult<TUser>(session.Get<TUser>(userId));
			}
		}

		public Task<TUser> FindByNameAsync(string userName)
		{
			using (var session = getSession())
			{
				return Task.FromResult<TUser>(session.QueryOver<TUser>().Where(u => u.UserName == userName).SingleOrDefault());
			}
		}

		public Task UpdateAsync(TUser user)
		{
			using (var session = getSession())
			{
				using (var transaction = session.BeginTransaction())
				{
					session.Update(user);
					transaction.Commit();
				}
			}

			return Task.FromResult<Object>(null);
		}

		public void Dispose()
		{
			getSession().Dispose();
		}

		public Task<int> GetAccessFailedCountAsync(TUser user)
		{
			return Task.FromResult<int>(user.AccessFailedCount);
		}

		public Task<bool> GetLockoutEnabledAsync(TUser user)
		{
			return Task.FromResult<bool>(user.LockoutEnabled);
		}

		public Task<DateTimeOffset> GetLockoutEndDateAsync(TUser user)
		{
			return Task.FromResult<DateTimeOffset>(user.LockoutEndDateUtc.HasValue ? user.LockoutEndDateUtc.Value : DateTime.Now.AddMinutes(-1));
		}

		public Task<int> IncrementAccessFailedCountAsync(TUser user)
		{
			user.AccessFailedCount++;
			return Task.FromResult<int>(user.AccessFailedCount);
		}

		public Task ResetAccessFailedCountAsync(TUser user)
		{
			user.AccessFailedCount = 0;
			return Task.FromResult<Object>(null);
		}

		public Task SetLockoutEnabledAsync(TUser user, bool enabled)
		{
			user.LockoutEnabled = enabled;
			return Task.FromResult<Object>(null);
		}

		public Task SetLockoutEndDateAsync(TUser user, DateTimeOffset lockoutEnd)
		{
			user.LockoutEndDateUtc = lockoutEnd.UtcDateTime;
			return Task.FromResult<Object>(null);
		}

		public Task<string> GetPasswordHashAsync(TUser user)
		{
			return Task.FromResult<string>(user.PasswordHash);
		}

		public Task<bool> HasPasswordAsync(TUser user)
		{
			string passwordHash = GetPasswordHashAsync(user).Result;
			bool hasPassword = !String.IsNullOrEmpty(passwordHash);

			return Task.FromResult<bool>(hasPassword);
		}

		public Task SetPasswordHashAsync(TUser user, string passwordHash)
		{
			user.PasswordHash = passwordHash;
			return Task.FromResult<Object>(null);
		}

		public Task<bool> GetTwoFactorEnabledAsync(TUser user)
		{
			return Task.FromResult<bool>(user.TwoFactorEnabled);
		}

		public Task SetTwoFactorEnabledAsync(TUser user, bool enabled)
		{
			user.TwoFactorEnabled = enabled;
			return Task.FromResult<Object>(null);
		}

		public Task<string> GetPhoneNumberAsync(TUser user)
		{
			return Task.FromResult<string>(user.PhoneNumber);
		}

		public Task<bool> GetPhoneNumberConfirmedAsync(TUser user)
		{
			return Task.FromResult<bool>(user.PhoneNumberConfirmed);
		}

		public Task SetPhoneNumberAsync(TUser user, string phoneNumber)
		{
			user.PhoneNumber = phoneNumber;
			return Task.FromResult<Object>(null);
		}

		public Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed)
		{
			user.PhoneNumberConfirmed = confirmed;
			return Task.FromResult<Object>(null);
		}

		public Task AddLoginAsync(TUser user, UserLoginInfo login)
		{
			throw new NotImplementedException();
		}

		public Task<TUser> FindAsync(UserLoginInfo login)
		{
			throw new NotImplementedException();
		}

		public Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user)
		{
			//not implementing it for now
			List<UserLoginInfo> logins = new List<UserLoginInfo>();
			return Task.FromResult<IList<UserLoginInfo>>(logins);
		}

		public Task RemoveLoginAsync(TUser user, UserLoginInfo login)
		{
			throw new NotImplementedException();
		}

		public Task<TUser> FindByEmailAsync(string email)
		{
			using (var session = getSession())
			{
				return Task.FromResult<TUser>(session.QueryOver<TUser>().Where(u => u.Email == email).SingleOrDefault());
			}
		}

		public Task<string> GetEmailAsync(TUser user)
		{
			return Task.FromResult<string>(user.Email);
		}

		public Task<bool> GetEmailConfirmedAsync(TUser user)
		{
			return Task.FromResult<bool>(user.EmailConfirmed);
		}

		public Task SetEmailAsync(TUser user, string email)
		{
			user.Email = email;
			return Task.FromResult<Object>(null);
		}

		public Task SetEmailConfirmedAsync(TUser user, bool confirmed)
		{
			user.EmailConfirmed = confirmed;
			return Task.FromResult<Object>(null);
		}

		public Task<string> GetSecurityStampAsync(TUser user)
		{
			return Task.FromResult<string>(user.SecurityStamp);
		}

		public Task SetSecurityStampAsync(TUser user, string stamp)
		{
			user.SecurityStamp = stamp;
			return Task.FromResult<Object>(null);
		}

		//======================= not supported interfaces ===============================


		public Task AddClaimAsync(TUser user, System.Security.Claims.Claim claim)
		{
			throw new NotImplementedException();
		}

		public Task<IList<System.Security.Claims.Claim>> GetClaimsAsync(TUser user)
		{
			List<System.Security.Claims.Claim> claims = new List<System.Security.Claims.Claim>();
			return Task.FromResult<IList<System.Security.Claims.Claim>>(claims);
		}

		public Task RemoveClaimAsync(TUser user, System.Security.Claims.Claim claim)
		{
			throw new NotImplementedException();
		}

		public Task AddToRoleAsync(TUser user, string roleName)
		{
			using (var session = getSession())
			{
				using (var transaction = session.BeginTransaction())
				{
					var role = session.QueryOver<SparIdentityRole>().Where(rr => rr.Name == roleName).SingleOrDefault();
					var identityRole = new SparIdentityUserRole() { UserId = user.Id, RoleId = role.Id };
					session.Save(identityRole);
					transaction.Commit();
				}
			}

			return Task.FromResult<Object>(null);
		}

		public Task<IList<string>> GetRolesAsync(TUser user)
		{
			//ToDo: to implement
			List<string> roles = new List<string>();
			return Task.FromResult<IList<string>>(roles);
		}

		public Task<bool> IsInRoleAsync(TUser user, string roleName)
		{
			SparIdentityUserRole userRole = null;
			using (var session = getSession())
			{
				var role = session.QueryOver<SparIdentityRole>().Where(rr => rr.Name == roleName).SingleOrDefault();
				userRole = session.QueryOver<SparIdentityUserRole>().Where(ur => ur.UserId == user.Id && ur.RoleId == role.Id).SingleOrDefault();
			}

			return Task.FromResult<bool>(userRole != null);
		}

		public Task RemoveFromRoleAsync(TUser user, string roleName)
		{
			throw new NotImplementedException();
		}

		public IQueryable<TUser> Users
		{
			get { throw new NotImplementedException(); }
		}
	}
}
