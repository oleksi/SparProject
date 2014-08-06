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

		public virtual string getProfileThumbnailFileName(int thumbSize)
		{
			return getProfileThumbnailFileName(thumbSize, false);
		}

		public virtual string getProfileThumbnailFileName(int thumbSize, bool noneAnonymousMode)
		{
			string profileThumbnailFileName = (ProfilePictureUploaded == true || noneAnonymousMode == true) ? this.SparIdentityUser.Id : "anonymous-photo";
			return String.Format("{0}-{1}X{1}.jpg", profileThumbnailFileName, thumbSize);
		}

		public virtual int getFighterAge()
		{
			return Convert.ToInt32(((double)(DateTime.Now - this.DateOfBirth).Days / 365.2425));
		}

		public virtual string GetHimOrHer(bool useTitleCase)
		{
			if (useTitleCase == true)
				return (Sex == true) ? "Him" : "Her";
			else
				return (Sex == true) ? "him" : "her";
		}
	}
}
