using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SparWeb.Controllers
{
    public class FighterController : Controller
    {
        //
        // GET: /Fighter/
        public ActionResult Index()
        {
            return View();
        }

		[Authorize]
		public ActionResult SparConfirmation(string ID)
		{
			string aaa = ID;

			return View();
		}
	}
}