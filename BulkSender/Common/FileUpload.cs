using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BulkSender.Common
{
	[ModelBinder(typeof(ModelBinder))]
	public class FileUpload
	{
		public string FileName { get; set; }
		public string Guid { get; set; }
		public Stream InputStream { get; set; }

		public class ModelBinder : IModelBinder
		{
			public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
			{
				var request = controllerContext.RequestContext.HttpContext.Request;
				var formUpload = request.Files.Count > 0;
				var xFileName = request.Headers["X-File-Name"];
				var qqFile = request["qqfile"];
				var formFilename = formUpload ? request.Files[0].FileName : null;
				var guid = request["qqguid"];
				var fileUpload = new FileUpload
				{
					FileName = xFileName ?? qqFile ?? formFilename,
					InputStream = formUpload ? request.Files[0].InputStream : request.InputStream,
					Guid = guid ?? string.Empty
				};
				return fileUpload;
			}
		}

		public static bool GenerateImageThumbnail(Image oImage, string localFilePath, int nMaxWidth = 0, int nMaxHeight = 0, int yCoord = 0, int xCoord = 0)
		{
			try
			{
				var ratioX = (double)nMaxWidth / oImage.Width;
				var ratioY = (double)nMaxHeight / oImage.Height;
				var ratio = Math.Max(ratioX, ratioY);
				var newWidth = Convert.ToInt32(oImage.Width * ratio);
				var newHeight = Convert.ToInt32(oImage.Height * ratio);
				if ((nMaxWidth == 0 && nMaxHeight == 0) || (nMaxHeight >= oImage.Height || nMaxWidth >= oImage.Width))
				{
					newWidth = oImage.Width;
					newHeight = oImage.Height;
				}
				var image = new Bitmap(newWidth, newHeight, PixelFormat.Format24bppRgb);
				var oGraphic = Graphics.FromImage(image);
				oGraphic.Clear(Color.Transparent);
				oGraphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
				oGraphic.SmoothingMode = SmoothingMode.HighQuality;
				oGraphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
				oGraphic.CompositingQuality = CompositingQuality.Default;
				oGraphic.DrawImage(oImage, new Rectangle(xCoord, yCoord, newWidth, newHeight));
				//if (File.Exists(HttpContext.Current.Server.MapPath(localFilePath)))
				//	File.Delete(HttpContext.Current.Server.MapPath(localFilePath));
				image.Save(HttpContext.Current.Server.MapPath(localFilePath));
				image.Dispose();
				oImage.Dispose();
				oGraphic.Dispose();
				image = null;
				return true;
			}
			catch (Exception ex)
			{
				return false;
			}
		}

	}
}