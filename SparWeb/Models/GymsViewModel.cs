using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SparWeb.Models
{
	public class GymsViewModel
	{
		public List<GymViewModel> GymsList { get; set; }

		public string SearchState { get; set; }
		public string SearchStateLong { get; set; }

		public int PagesCount { get; set; }
		public int PageNumber { get; set; }

		public string FilterParamsQueryString { get; set; }
	}
}