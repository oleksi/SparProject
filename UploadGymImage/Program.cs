using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using SparData;
using SparModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace UploadGymImage
{
	class Program
	{
		static void Main(string[] args)
		{
			int gymId = Convert.ToInt32(args[0]);
			
			string filePath = args[1];
			using (var fileStream = File.OpenRead(filePath))
			{

				string fileName = "";
				Gym gym = null;
				try
				{
					CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["SparStorage"].ConnectionString);
					CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

					CloudBlobContainer container = blobClient.GetContainerReference("images");
					if (container.Exists() == false)
					{
						container.CreateIfNotExists();
						container.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
					}

					var gymRepo = new GymRepository();
					gym = gymRepo.GetGymById(gymId);

					//optimizing and saving uploaded pic in full size
					string origFileName = String.Format("{0}-orig.jpg", gymId);
					Bitmap bitmap = new Bitmap(fileStream);
					saveProfilePic(bitmap, origFileName, container);

					//cropping the pic into square
					Rectangle cropRect = Rectangle.Empty;
					if (bitmap.Width > bitmap.Height)
						cropRect = new Rectangle((int)(bitmap.Width - bitmap.Height) / 2, 0, bitmap.Height, bitmap.Height);
					else
						cropRect = new Rectangle(0, (int)(bitmap.Height - bitmap.Width) / 2, bitmap.Width, bitmap.Width);
					bitmap = cropImage(bitmap, cropRect);

					//resizing and saving account thumbnail version
					bitmap = new Bitmap(bitmap, 250, 250);
					fileName = gym.GetGymThumbnailFileName(250);
					saveProfilePic(bitmap, fileName, container);

					bitmap = new Bitmap(bitmap, 150, 150);
					saveProfilePic(bitmap, gym.GetGymThumbnailFileName(150), container);

					gym.GymPictureUploaded = true;
					gymRepo.SaveGym(gym);
				}
				catch (Exception ex)
				{
					Console.WriteLine("Error: " + ex.Message);
				}
			}
		}

		private static void saveProfilePic(Bitmap bitmap, string fileName, CloudBlobContainer container)
		{
			CloudBlockBlob blockBlob = container.GetBlockBlobReference(String.Format("GymPics/{0}", fileName));

			//converting to JPG
			ImageCodecInfo jgpEncoder = getEncoder(ImageFormat.Jpeg);
			EncoderParameters myEncoderParameters = new EncoderParameters(1);
			myEncoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 70L);
			byte[] byteArray = null;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				bitmap.Save(memoryStream, jgpEncoder, myEncoderParameters);
				memoryStream.Close();
				byteArray = memoryStream.ToArray();
			}

			//saving to the blob storage
			using (MemoryStream memoryStream = new MemoryStream(byteArray))
			{
				blockBlob.UploadFromStream(memoryStream);
			}
		}

		private static Bitmap cropImage(Bitmap src, Rectangle cropRect)
		{
			Bitmap target = new Bitmap(cropRect.Width, cropRect.Height);

			using (Graphics g = Graphics.FromImage(target))
			{
				g.DrawImage(src, new Rectangle(0, 0, target.Width, target.Height),
								 cropRect,
								 GraphicsUnit.Pixel);
			}

			return target;
		}

		private static ImageCodecInfo getEncoder(ImageFormat format)
		{
			ImageCodecInfo encoder = null;
			ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

			foreach (ImageCodecInfo codec in codecs)
			{
				if (codec.FormatID == format.Guid)
				{
					encoder = codec;
					break;
				}
			}
			return encoder;
		}
	}
}
