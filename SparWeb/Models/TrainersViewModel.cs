using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SparWeb.Models
{
	public class TrainersViewModel
	{
		public List<AccountTrainerViewModel> TrainersList { get; set; }

		public string SearchState { get; set; }
		public string SearchStateLong { get; set; }

		public int PagesCount { get; set; }
		public int PageNumber { get; set; }

		public string FilterParamsQueryString { get; set; }
	}
}