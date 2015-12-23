using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SparWeb.Models
{
	public class GymViewModel
	{
		public virtual int Id { get; set; }
		public string Name { get; set; }
		public string StreetAddress { get; set; }
		public string City { get; set; }
		public string State { get; set; }
		public string ZipCode { get; set; }

		[Display(Name = "Phone:")]
		public string Phone { get; set; }

		[Display(Name = "Address:")]
		public string Address 
		{
 			get
			{
				return String.Format("{0}<br />{1}, {2} {3}", StreetAddress, City, State, ZipCode);
			}
		}

		public string GymPictureFile { get; set; }

		public string GymUrl
		{
			get
			{
				return String.Format("/gyms/{0}/{1}", Util.States[State], Name);
			}
		}

		public List<AccountFighterViewModel> FightersList { get; set; }

		public GymViewModel()
		{
			FightersList = new List<AccountFighterViewModel>();
		}
	}
}
