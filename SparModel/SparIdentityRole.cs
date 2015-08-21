using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace SparModel
{
	public class SparIdentityRole : IRole
	{
		public virtual string Id { get; set; }
		public virtual string Name { get; set; }
	}
}
