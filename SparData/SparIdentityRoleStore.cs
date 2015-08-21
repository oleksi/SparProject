using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using NHibernate;
using SparModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SparData
{
	public class SparIdentityRoleStore<TRole> : IRoleStore<TRole, string> where TRole : SparIdentityRole
	{
		private static ISession getSession()
		{
			return SessionProvider.SessionFactory.OpenSession();
		}

		public Task CreateAsync(TRole role)
		{
			using (var session = getSession())
			{
				using (var transaction = session.BeginTransaction())
				{
					session.Save(role);
					transaction.Commit();
				}
			}

			return Task.FromResult<Object>(null);
		}

		public Task DeleteAsync(TRole role)
		{
			using (var session = getSession())
			{
				using (var transaction = session.BeginTransaction())
				{
					session.Delete(role);
					transaction.Commit();
				}
			}

			return Task.FromResult<Object>(null);
		}

		public Task<TRole> FindByIdAsync(string roleId)
		{
			using (var session = getSession())
			{
				return Task.FromResult<TRole>(session.Get<TRole>(roleId));
			}
		}

		public Task<TRole> FindByNameAsync(string roleName)
		{
			using (var session = getSession())
			{
				return Task.FromResult<TRole>(session.QueryOver<TRole>().Where(u => u.Name == roleName).SingleOrDefault());
			}
		}

		public Task UpdateAsync(TRole role)
		{
			using (var session = getSession())
			{
				using (var transaction = session.BeginTransaction())
				{
					session.Update(role);
					transaction.Commit();
				}
			}

			return Task.FromResult<Object>(null);
		}

		public void Dispose()
		{
			getSession().Dispose();
		}
	}
}
