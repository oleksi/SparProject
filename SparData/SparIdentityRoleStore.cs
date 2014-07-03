using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SparData
{
	public class SparIdentityRoleStore<TRole> : IRoleStore<TRole, string> where TRole : IdentityRole
	{
		private static ISession getSession()
		{
			return SessionProvider.SessionFactory.OpenSession();
		}

		public Task CreateAsync(TRole role)
		{
			throw new NotImplementedException();
		}

		public Task DeleteAsync(TRole role)
		{
			throw new NotImplementedException();
		}

		public Task<TRole> FindByIdAsync(string roleId)
		{
			throw new NotImplementedException();
		}

		public Task<TRole> FindByNameAsync(string roleName)
		{
			throw new NotImplementedException();
		}

		public Task UpdateAsync(TRole role)
		{
			throw new NotImplementedException();
		}

		public void Dispose()
		{
			throw new NotImplementedException();
		}
	}
}
