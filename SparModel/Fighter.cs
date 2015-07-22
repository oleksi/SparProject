using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SparModel
{
    public class Fighter : Member
    {
		public virtual bool Sex { get; set; } //1 - Male; 0 - Female
		public virtual double Height { get; set; }
		public virtual double Weight { get; set; }
		public virtual bool IsSouthpaw { get; set; }
		public virtual int NumberOfAmateurFights { get; set; }
		public virtual int NumberOfProFights { get; set; }
		public virtual Gym Gym { get; set; }

		public virtual string GetHimOrHer(bool useTitleCase)
		{
			if (useTitleCase == true)
				return (Sex == true) ? "Him" : "Her";
			else
				return (Sex == true) ? "him" : "her";
		}

		public virtual string GetHeOrShe(bool useTitleCase)
		{
			if (useTitleCase == true)
				return (Sex == true) ? "He" : "She";
			else
				return (Sex == true) ? "he" : "she";
		}

		public virtual int NumberOfFights
		{
			get { return this.NumberOfAmateurFights + this.NumberOfProFights; }
		}
	}
}
