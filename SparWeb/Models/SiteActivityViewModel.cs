using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SparWeb.Models
{
	public class SiteActivityViewModel
	{
		public SparActivityViewModel SparActivity { get; set; }
		public AccountFighterViewModel Fighter { get; set; }
		public DateTime ActivityDate { get; set; }
	}
}