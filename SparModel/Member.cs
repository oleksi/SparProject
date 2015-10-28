using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SparModel
{
	public class Member
	{
		public virtual int? Id { get; set; }
		public virtual string Name { get; set; }
		public virtual DateTime DateOfBirth { get; set; }
		public virtual string City { get; set; }
		public virtual string State { get; set; }
		public virtual DateTime InsertDate { get; set; }
		public virtual DateTime UpdateDate { get; set; }
		public virtual SparIdentityUser SparIdentityUser { get; set; }
		public virtual bool ProfilePictureUploaded { get; set; }

		public virtual string GetProfileThumbnailFileName(int thumbSize)
		{
			return GetProfileThumbnailFileName(thumbSize, false);
		}

		public virtual string GetProfileThumbnailFileName(int thumbSize, bool noneAnonymousMode)
		{
			return GetProfileThumbnailFileName(thumbSize, noneAnonymousMode, false);
		}

		public virtual string GetProfileThumbnailFileName(int thumbSize, bool noneAnonymousMode, bool dontAppendCachingBlocker)
		{
			string profileThumbnailFileName = "";
			if (ProfilePictureUploaded == true || noneAnonymousMode == true)
			{
				profileThumbnailFileName = this.SparIdentityUser.Id;
			}
			else
			{
				if (this is Fighter)
					profileThumbnailFileName += "anonymous-photo-fighter";
				else
					profileThumbnailFileName += "anonymous-photo-trainer";
			}

			return String.Format("{0}-{1}X{1}.jpg{2}", profileThumbnailFileName, thumbSize, (!dontAppendCachingBlocker && ProfilePictureUploaded)? ("?" + DateTime.Now.GetHashCode().ToString()) : "");
		}

		public virtual string GetProfilePictureFile(int thumbnailSize, string profilePicsUrl, string defaultImagesUrl)
		{
			return String.Format("{0}{1}", (this.ProfilePictureUploaded == true) ? profilePicsUrl : defaultImagesUrl, this.GetProfileThumbnailFileName(thumbnailSize));
		}

		public virtual int GetMemberAge()
		{
			DateTime now = DateTime.Today;

			int age = now.Year - this.DateOfBirth.Year;
			if (this.DateOfBirth > now.AddYears(-age)) age--;

			return age;
		}
	}
}
