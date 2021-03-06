﻿using System.Web;
using System.Web.Mvc;

namespace SparWeb
{
	public class FilterConfig
	{
		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			filters.Add(new RequreSecureConnectionFilter());
			filters.Add(new HandleErrorAttribute());
			filters.Add(new GlobalActionFilter(), 0);
		}
	}
}
