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
		DateLocationNegotiation = 2,
		Confirmed = 3,
		Canceled = 4
	}

	public class SparRequest
	{
		public virtual string Id { get; set; }
		public virtual SparRequestStatus Status { get; set; }
		public virtual Fighter RequestorFighter { get; set; }
		public virtual DateTime RequestDate { get; set; }
		public virtual Fighter OpponentFighter { get; set; }
		public virtual DateTime? SparDateTime { get; set; }
		public virtual Gym SparGym { get; set; }
		public virtual string SparNotes { get; set; }
		public virtual Fighter LastNegotiatorFighter { get; set; }
	}
}
