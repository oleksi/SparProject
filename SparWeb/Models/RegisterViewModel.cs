using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SparWeb.Models
{
	public class RegisterMainViewModel
	{
		public int? Mode { get; set; }

		public bool IsPopupMode { get; set; }

		public bool IsFromBlog { get; set; }
	}
}