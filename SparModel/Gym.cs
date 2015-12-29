using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SparModel
{
	public class Gym
	{
		public virtual int? Id { get; set; }
		public virtual string Name { get; set; }
		public virtual string StreetAddress { get; set; }
		public virtual string City { get; set; }
		public virtual string State { get; set; }
		public virtual string ZipCode { get; set; }
		public virtual string Phone { get; set; }
		public virtual DateTime InsertDate { get; set; }
		public virtual DateTime UpdateDate { get; set; }
		public virtual bool GymPictureUploaded { get; set; }

		public virtual string GetGymPictureFile(int thumbnailSize, string gymPicsUrl, string defaultImagesUrl)
		{
			return String.Format("{0}{1}", (this.GymPictureUploaded == true) ? gymPicsUrl : defaultImagesUrl, this.GetGymThumbnailFileName(thumbnailSize));
		}

		public virtual string GetGymThumbnailFileName(int thumbnailSize)
		{
			string gymThumbnailFileName = "";
			if (GymPictureUploaded == true)
			{
				gymThumbnailFileName = this.Id.ToString();
			}
			else
			{
				gymThumbnailFileName += "anonymous-photo-gym";
			}

			return String.Format("{0}-{1}X{1}.jpg", gymThumbnailFileName, thumbnailSize);
		}
	}
}
