﻿using System;
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
		public virtual string City { get; set; }
		public virtual string State { get; set; }
		public virtual double Height { get; set; }
		public virtual double Weight { get; set; }
		public virtual bool IsSouthpaw { get; set; }
		public virtual int NumberOfAmateurFights { get; set; }
		public virtual int NumberOfProFights { get; set; }
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

		public virtual string GetProfilePictureFile(int thumbnailSize, string profilePicsUrl, string defaultImagesUrl)
		{
			return String.Format("{0}{1}", (this.ProfilePictureUploaded == true) ? profilePicsUrl : defaultImagesUrl, this.getProfileThumbnailFileName(thumbnailSize));
		}

		public virtual int getFighterAge()
		{
			DateTime now = DateTime.Today;

			int age = now.Year - this.DateOfBirth.Year;
			if (this.DateOfBirth > now.AddYears(-age)) age--;

			return age;
		}

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
