using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SparModel
{
    public class Fighter
    {
		public virtual int? Id { get; set; }
		public virtual string Name { get; set; }
		public virtual bool Sex { get; set; } //1 - Male; 0 - Female
		public virtual DateTime DateOfBirth { get; set; }
		public virtual double Height { get; set; }
		public virtual double Weight { get; set; }
		public virtual int NumberOfFights { get; set; }
		public virtual Gym Gym { get; set; }
		public virtual DateTime InsertDate { get; set; }
		public virtual DateTime UpdateDate { get; set; }
		public virtual SparIdentityUser SparIdentityUser { get; set; }
		public virtual bool ProfilePictureUploaded { get; set; }
	}
}
