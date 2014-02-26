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
	public class SparIdentityUserStore<TUser> : IUserStore<TUser>, IUserPasswordStore<TUser>, IUserSecurityStampStore<TUser>, IDisposable where TUser : global::Microsoft.AspNet.Identity.EntityFramework.IdentityUser
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
	}
}
