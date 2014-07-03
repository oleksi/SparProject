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
			saveOrUpdateUser(user);

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
			saveOrUpdateUser(user);

			return Task.FromResult<Object>(null);
		}

		private void saveOrUpdateUser(TUser user)
		{
			using (var session = getSession())
			{
				using (var transaction = session.BeginTransaction())
				{
					session.SaveOrUpdate(user);
					transaction.Commit();
				}
			}
		}

		public void Dispose()
		{
			getSession().Dispose();
		}

		public Task<int> GetAccessFailedCountAsync(TUser user)
		{
			using (var session = getSession())
			{
				int accessFailedCount = session.QueryOver<TUser>().Where(u => u.Id == user.Id).Select(u => u.AccessFailedCount).SingleOrDefault<int>();
				return Task.FromResult<int>(accessFailedCount);
			}
		}

		public Task<bool> GetLockoutEnabledAsync(TUser user)
		{
			using (var session = getSession())
			{
				bool lockoutEnabled = session.QueryOver<TUser>().Where(u => u.Id == user.Id).Select(u => u.LockoutEnabled).SingleOrDefault<bool>();
				return Task.FromResult<bool>(lockoutEnabled);
			}
		}

		public Task<DateTimeOffset> GetLockoutEndDateAsync(TUser user)
		{
			using (var session = getSession())
			{
				DateTimeOffset? lockoutEndDate = session.QueryOver<TUser>().Where(u => u.Id == user.Id).Select(u => u.LockoutEndDateUtc).SingleOrDefault<DateTimeOffset?>();
				return Task.FromResult<DateTimeOffset>(lockoutEndDate.HasValue ? lockoutEndDate.Value : DateTime.Now.AddMinutes(-1));
			}
		}

		public Task<int> IncrementAccessFailedCountAsync(TUser user)
		{
			user.AccessFailedCount++;
			UpdateAsync(user);

			return Task.FromResult<int>(user.AccessFailedCount);
		}

		public Task ResetAccessFailedCountAsync(TUser user)
		{
			user.AccessFailedCount = 0;
			return UpdateAsync(user);
		}

		public Task SetLockoutEnabledAsync(TUser user, bool enabled)
		{
			user.LockoutEnabled = enabled;
			return UpdateAsync(user);
		}

		public Task SetLockoutEndDateAsync(TUser user, DateTimeOffset lockoutEnd)
		{
			user.LockoutEndDateUtc = lockoutEnd.UtcDateTime;
			return UpdateAsync(user);
		}

		public Task<string> GetPasswordHashAsync(TUser user)
		{
			using (var session = getSession())
			{
				string passwordHash = session.QueryOver<TUser>().Where(u => u.Id == user.Id).Select(u => u.PasswordHash).SingleOrDefault<string>();
				return Task.FromResult<string>(passwordHash);
			}
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
			return UpdateAsync(user);
		}

		public Task<bool> GetTwoFactorEnabledAsync(TUser user)
		{
			using (var session = getSession())
			{
				bool twoFactorEnabled = session.QueryOver<TUser>().Where(u => u.Id == user.Id).Select(u => u.TwoFactorEnabled).SingleOrDefault<bool>();
				return Task.FromResult<bool>(twoFactorEnabled);
			}
		}

		public Task SetTwoFactorEnabledAsync(TUser user, bool enabled)
		{
			user.TwoFactorEnabled = enabled;
			return UpdateAsync(user);
		}

		public Task<string> GetPhoneNumberAsync(TUser user)
		{
			using (var session = getSession())
			{
				string phoneNumber = session.QueryOver<TUser>().Where(u => u.Id == user.Id).Select(u => u.PhoneNumber).SingleOrDefault<string>();
				return Task.FromResult<string>(phoneNumber);
			}
		}

		public Task<bool> GetPhoneNumberConfirmedAsync(TUser user)
		{
			using (var session = getSession())
			{
				bool phoneNumberConfirmed = session.QueryOver<TUser>().Where(u => u.Id == user.Id).Select(u => u.PhoneNumberConfirmed).SingleOrDefault<bool>();
				return Task.FromResult<bool>(phoneNumberConfirmed);
			}
		}

		public Task SetPhoneNumberAsync(TUser user, string phoneNumber)
		{
			user.PhoneNumber = phoneNumber;
			return UpdateAsync(user);
		}

		public Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed)
		{
			user.PhoneNumberConfirmed = confirmed;
			return UpdateAsync(user);
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
			using (var session = getSession())
			{
				string email = session.QueryOver<TUser>().Where(u => u.Id == user.Id).Select(u => u.Email).SingleOrDefault<string>();
				return Task.FromResult<string>(email);
			}
		}

		public Task<bool> GetEmailConfirmedAsync(TUser user)
		{
			using (var session = getSession())
			{
				bool emailConfirmed = session.QueryOver<TUser>().Where(u => u.Id == user.Id).Select(u => u.EmailConfirmed).SingleOrDefault<bool>();
				return Task.FromResult<bool>(emailConfirmed);
			}
		}

		public Task SetEmailAsync(TUser user, string email)
		{
			user.Email = email;
			return UpdateAsync(user);
		}

		public Task SetEmailConfirmedAsync(TUser user, bool confirmed)
		{
			user.EmailConfirmed = confirmed;
			return UpdateAsync(user);
		}

		public Task<string> GetSecurityStampAsync(TUser user)
		{
			using (var session = getSession())
			{
				string securityStamp = session.QueryOver<TUser>().Where(u => u.Id == user.Id).Select(u => u.SecurityStamp).SingleOrDefault<string>();
				return Task.FromResult<string>(securityStamp);
			}
		}

		public Task SetSecurityStampAsync(TUser user, string stamp)
		{
			user.SecurityStamp = stamp;
			return UpdateAsync(user);
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
			throw new NotImplementedException();
		}

		public Task<IList<string>> GetRolesAsync(TUser user)
		{
			//ToDo: to implement
			List<string> roles = new List<string>();
			return Task.FromResult<IList<string>>(roles);
		}

		public Task<bool> IsInRoleAsync(TUser user, string roleName)
		{
			throw new NotImplementedException();
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
