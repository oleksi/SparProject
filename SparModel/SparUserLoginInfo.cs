using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace SparModel
{
	public class SparUserLoginInfo
	{
		public virtual UserLoginInfoIdentifier UserLoginInfoIdentifier { get; set; }
	}

	[Serializable]
	public class UserLoginInfoIdentifier
	{
		public UserLoginInfoIdentifier() { }

		public virtual string UserId { get; set; }
		public virtual string LoginProvider { get; set; }
		public virtual string ProviderKey { get; set; }

		public UserLoginInfoIdentifier(UserLoginInfo userLoginInfo)
		{
			this.LoginProvider = userLoginInfo.LoginProvider;
			this.ProviderKey = userLoginInfo.ProviderKey;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || (obj is UserLoginInfoIdentifier) == false)
				return false;

			UserLoginInfoIdentifier sparUserLoginInfo = obj as UserLoginInfoIdentifier;

			return (this.UserId == sparUserLoginInfo.UserId && this.LoginProvider == sparUserLoginInfo.LoginProvider && this.ProviderKey == sparUserLoginInfo.ProviderKey);
		}

		public override int GetHashCode()
		{
			return String.Format("{0}|{1}", LoginProvider, ProviderKey).GetHashCode();
		}
	}
}
