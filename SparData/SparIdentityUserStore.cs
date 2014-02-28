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
	public class SparIdentityUserStore<TUser> : IUserStore<TUser>, IUserPasswordStore<TUser>, IUserSecurityStampStore<TUser>, IUserLoginStore<TUser>, IDisposable where TUser : SparIdentityUser
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
					session.SaveOrUpdate(user);
					transaction.Commit();
				}
			}

			return Task.FromResult<Object>(null);
		}

		public void Dispose()
		{
			getSession().Dispose();
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
			return Task.FromResult<Object>(null);
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
			return Task.FromResult<Object>(null);
		}

		public Task AddLoginAsync(TUser user, UserLoginInfo login)
		{
			SparUserLoginInfo loginInfo = new SparUserLoginInfo() { UserLoginInfoIdentifier = new UserLoginInfoIdentifier(login) };
			loginInfo.UserLoginInfoIdentifier.UserId = user.Id;

			if (user.Logins == null)
				user.Logins = new List<SparUserLoginInfo>();
			user.Logins.Add(loginInfo);

			return UpdateAsync(user);
		}

		public Task<TUser> FindAsync(UserLoginInfo login)
		{
			using (var session = getSession())
			{
				string userId = session.QueryOver<SparUserLoginInfo>().Where(li => li.UserLoginInfoIdentifier.LoginProvider == login.LoginProvider && li.UserLoginInfoIdentifier.ProviderKey == login.ProviderKey).Select(li => li.UserLoginInfoIdentifier.UserId).SingleOrDefault<string>();
				if (String.IsNullOrEmpty(userId) == false)
					return FindByIdAsync(userId);
				else
					return Task.FromResult<TUser>(null);
			}
		}

		public Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user)
		{
			List<UserLoginInfo> logins = new List<UserLoginInfo>();

			foreach (SparUserLoginInfo sparUserLoginInfo in user.Logins)
				logins.Add(new UserLoginInfo(sparUserLoginInfo.UserLoginInfoIdentifier.LoginProvider, sparUserLoginInfo.UserLoginInfoIdentifier.ProviderKey));

			return Task.FromResult<IList<UserLoginInfo>>(logins);
		}

		public Task RemoveLoginAsync(TUser user, UserLoginInfo login)
		{
			SparUserLoginInfo sparUserLoginInfo = user.Logins.FirstOrDefault(li => li.UserLoginInfoIdentifier.LoginProvider == login.LoginProvider && li.UserLoginInfoIdentifier.ProviderKey == login.ProviderKey);
			if (sparUserLoginInfo == null)
				return Task.FromResult<Object>(null);

			user.Logins.Remove(sparUserLoginInfo);
			UpdateAsync(user);

			return Task.FromResult<Object>(null);
		}
	}
}
