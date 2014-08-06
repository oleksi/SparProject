using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SparModel
{
	public enum SparRequestStatus
	{
		Requested = 1,
		Confirmed = 2
	}

	public class SparRequest
	{
		public virtual string Id { get; set; }
		public virtual SparRequestStatus Status { get; set; }
		public virtual Fighter RequestorFighter { get; set; }
		public virtual DateTime RequestDate { get; set; }
		public virtual Fighter OpponentFighter { get; set; }
	}
}
