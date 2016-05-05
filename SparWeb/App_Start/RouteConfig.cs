using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SparWeb
{
	public class RouteConfig
	{
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.MapRoute(
				name: "About",
				url: "about",
				defaults: new { controller = "Home", action = "About" }
			);

			routes.MapRoute(
				name: "Contact",
				url: "contact",
				defaults: new { controller = "Home", action = "Contact" }
			);

			routes.MapRoute(
				name: "SiteMap",
				url: "SiteMap",
				defaults: new { controller = "Home", action = "SiteMap" }
			);

			routes.MapRoute(
				name: "Error",
				url: "Error",
				defaults: new { controller = "Home", action = "Error" }
			);

			routes.MapRoute(
				name: "Fighters",
				url: "fighters/{state}/{name}",
				defaults: new { controller = "Fighters", action = "Fighter" }
			);

			routes.MapRoute(
				name: "GymsByState",
				url: "gyms/{state}/",
				defaults: new { controller = "Gyms", action = "Index" }
			);

			routes.MapRoute(
				name: "Gyms",
				url: "gyms/{state}/{name}",
				defaults: new { controller = "Gyms", action = "Gym" }
			);

			routes.MapRoute(
				name: "TrainersByState",
				url: "trainers/{state}/",
				defaults: new { controller = "Trainers", action = "Index" }
			);

			routes.MapRoute(
				name: "Trainers",
				url: "trainers/{state}/{name}",
				defaults: new { controller = "Trainers", action = "Trainer" }
			);

			routes.MapRoute(
				name: "Default",
				url: "{controller}/{action}/{id}",
				defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
			);
		}
	}
}
