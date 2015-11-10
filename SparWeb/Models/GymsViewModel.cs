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

		[Display(Name = "Number of fights")]
		public string SearchState { get; set; }

		public int PagesCount { get; set; }
		public int PageNumber { get; set; }

		public string FilterParamsQueryString { get; set; }
	}
}