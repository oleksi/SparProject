using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SparModel
{
	public class Gym
	{
		public virtual int? Id { get; set; }
		public virtual string Name { get; set; }
		public virtual string StreetAddress { get; set; }
		public virtual string City { get; set; }
		public virtual string State { get; set; }
		public virtual string ZipCode { get; set; }
		public virtual string Phone { get; set; }
		public virtual DateTime InsertDate { get; set; }
		public virtual DateTime UpdateDate { get; set; }
	}
}
