using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SparWeb.Models
{
	public class SparNoteViewModel
	{
		public string ProfilePictureFile { get; set; }
		public string Name { get; set; }
		public DateTime NoteDate { get; set; }
		public string SparNotes { get; set; }
	}
}