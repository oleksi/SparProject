using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SparModel
{
	public class Trainer : Member
	{
		public virtual Gym Gym { get; set; }
		public virtual string PhoneNumber { get; set; }
		public virtual string Website { get; set; }
		public virtual decimal Rate { get; set; }
		public virtual string Notes { get; set; }
	}
}
