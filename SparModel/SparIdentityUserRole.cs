using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;

namespace SparModel
{
	public class SparIdentityUserRole : IdentityUserRole
	{
		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;

			var ur = obj as SparIdentityUserRole;
			if (ur == null)
				return false;

			return (ur.UserId == UserId && ur.RoleId == RoleId);
		}

		public override int GetHashCode()
		{
			return String.Format("{0}|{1}", UserId, RoleId).GetHashCode();
		}
	}
}
