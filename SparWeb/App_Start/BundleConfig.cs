﻿using System.Web;
using System.Web.Optimization;

namespace SparWeb
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

			bundles.Add(new ScriptBundle("~/bundles/jquerycookie").Include(
						"~/Scripts/jquery.cookie*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

			bundles.Add(new ScriptBundle("~/bundles/dropzone").Include(
					  "~/Scripts/dropzone.js",
					  "~/Scripts/profile-picture.js"));

			bundles.Add(new ScriptBundle("~/bundles/bootstrap-datepicker").Include(
					  "~/Scripts/bootstrap-datepicker.js", "~/Scripts/moment*"));


			bundles.Add(new StyleBundle("~/Styles/css").Include(
                      "~/Content/css/bootstrap.css",
                      "~/Content/css/site.css"));

			bundles.Add(new StyleBundle("~/Styles/css/dropzone").Include(
					  "~/Content/css/dropzone.css"));

			bundles.Add(new StyleBundle("~/Styles/css/bootstrap-datepicker").Include(
					  "~/Content/css/bootstrap-datepicker*"));		
		}
    }
}
