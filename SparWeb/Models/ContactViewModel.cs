using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SparWeb.Models
{
	public class ContactViewModel
	{
		[Required]
		public string Name { get; set; }

		[Required]
		public string Email { get; set; }
		
		[Required]
		public string Message { get; set; }
	}
}