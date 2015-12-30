using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SparWeb
{
	public class GlobalActionFilter : ActionFilterAttribute
	{
		public override void OnResultExecuting(ResultExecutingContext filterContext)
		{
			filterContext.Controller.ViewBag.AgeRange = Util.AgeRangeMap;
			filterContext.Controller.ViewBag.WeightClassMap = Util.WeightClassMap;
			filterContext.Controller.ViewBag.HeightToCentimetersMap = Util.HeightToCentimetersMap;
			filterContext.Controller.ViewBag.NumberOfFights = Util.NumberOfFights;
			filterContext.Controller.ViewBag.States = Util.States;
		}
	}
}