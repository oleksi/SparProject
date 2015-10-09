using SparWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SparWeb.Controllers
{
	public class RandomFightersWidgetViewModel
	{
		public List<AccountFighterViewModel> FightersList { get; set; }
		public string BackgroundColor { get; set; }
		public string ForegroundColor { get; set; }
	}
}