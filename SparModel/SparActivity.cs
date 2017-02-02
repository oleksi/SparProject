using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SparModel
{
	public class SparActivity
	{
		public string SparRequestId { get; set; }
		public int StatusId { get; set; }
		public int RequestorFighterId { get; set; }
		public DateTime RequestDate { get; set; }
		public int OpponentFighterId { get; set; }
		public int LastNegotiatorFighterId { get; set; }
		public DateTime LastUpdateDate { get; set; }
	}
}
