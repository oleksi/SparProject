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
		public virtual string UserId { get; set; }
		public virtual string LoginProvider { get; set; }
		public virtual string ProviderKey { get; set; }

		public SparUserLoginInfo() { }

		public SparUserLoginInfo(UserLoginInfo userLoginInfo)
		{
			this.LoginProvider = userLoginInfo.LoginProvider;
			this.ProviderKey = userLoginInfo.ProviderKey;
		}
	}
}
