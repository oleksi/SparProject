using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SparModel
{
	public class SparNote
	{
		public virtual int MemberId { get; set; }
		public virtual DateTime NoteDate { get; set; }
		public virtual string SparNotes { get; set; }
	}
}
