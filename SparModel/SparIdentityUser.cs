using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections;

namespace SparModel
{
	public class SparIdentityUser : IdentityUser
	{
		private string m_Discriminator = "SparIdentityUser";
		public virtual string Discriminator 
		{ 
			get 
			{
				return m_Discriminator; 
			}

			set
			{
				m_Discriminator = value;
			}
		}

		public virtual SparUserLoginInfo Login { get; set; }
	}
}
