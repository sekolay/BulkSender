using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using BulkSender.Common;
using BulkSender.Entity;
using BulkSender.Enum;
using BulkSender.Model;

namespace BulkSender.Controllers
{
    public class UploadController : Base
    {
		[RequiredAuth]
		public ResultType Delete(string fileName)
		{
			if (System.IO.File.Exists(System.Web.HttpContext.Current.Server.MapPath("~/Uploads/images/" + fileName)))
				System.IO.File.Delete(System.Web.HttpContext.Current.Server.MapPath("~/Uploads/images/" + fileName));
			return ResultType.Success;
		}

		[RequiredAuth]
		public ActionResult Index()
		{
			ViewBag.List = GetFilesList();
			return View();
		}

		[RequiredAuth]
		public ActionResult Post()
		{
			return View();
		}

		[HttpPost]
		[RequiredAuth]
		public JsonResult Post(FileUpload fileUpload)
		{
			var json = new JsonDataModel();
			json.result = ResultType.UnSuccess;
			json.message = "Dosya yüklenemedi";
			if (!(new Regex(@"(.*?)\.(jpg|jpeg|png|gif|bmp|tiff|tif)$", RegexOptions.IgnoreCase)).IsMatch(fileUpload.FileName))
				json.message = "Geçersiz dosya tipi";
			else if (fileUpload.InputStream.Length == 0)
				json.message = "Dosya Seçilmedi";
			else if (fileUpload.InputStream.Length > 0)
			{
				var maxsizeLarge = SettingManager.GetValue("ImageLargeMaxsize").ToString().Split('x');
				var maxsizeMedium = SettingManager.GetValue("ImageMediumMaxsize").ToString().Split('x');
				var maxsizeSmall = SettingManager.GetValue("ImageSmallMaxsize").ToString().Split('x');
				var imageFilePath = "/Uploads/images/" + fileUpload.FileName + "";
				var imageFilePathL = "/Uploads/images/(L) " + fileUpload.FileName + "";
				var imageFilePathM = "/Uploads/images/(M) " + fileUpload.FileName + "";
				var imageFilePathS = "/Uploads/images/(S) " + fileUpload.FileName + "";
				var result = FileUpload.GenerateImageThumbnail(Image.FromStream(fileUpload.InputStream), imageFilePath);
				result = FileUpload.GenerateImageThumbnail(Image.FromStream(fileUpload.InputStream), imageFilePathL, Convert.ToInt32(maxsizeLarge[0]), Convert.ToInt32(maxsizeLarge[1]));
				result = FileUpload.GenerateImageThumbnail(Image.FromStream(fileUpload.InputStream), imageFilePathM, Convert.ToInt32(maxsizeMedium[0]), Convert.ToInt32(maxsizeMedium[1]));
				result = FileUpload.GenerateImageThumbnail(Image.FromStream(fileUpload.InputStream), imageFilePathS, Convert.ToInt32(maxsizeSmall[0]), Convert.ToInt32(maxsizeSmall[1]));
				fileUpload.InputStream.Close();
				fileUpload.InputStream.Dispose();
				if (result)
				{
					json.result = ResultType.Success;
					json.message = fileUpload.FileName;
				}
			}
			return Json(new { json }, JsonRequestBehavior.AllowGet);
		}

		private List<FileModel> GetFilesList()
		{
			var list = new List<FileModel>();
			string[] filePaths = Directory.GetFiles(Server.MapPath("~/Uploads/images/"), "*.*");
			foreach (var item in filePaths) {
				Bitmap i = new Bitmap(item);
				FileInfo f = new FileInfo(item);
				list.Add(new FileModel { CreateDate = f.CreationTime, Resolution = "" + i.Width + "x" + i.Height + "", Name = item.Replace(Server.MapPath("~/Uploads/images/"), ""), SizeKb = f.Length /1024 });
			}
			return list;
		}

    }
}
