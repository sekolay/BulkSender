using BulkSender.Common;
using BulkSender.Entity;
using EntityFramework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BulkSender.Controllers
{
    public class SettingController : Base
    {
		[RequiredAuth]
        public ActionResult Index()
        {
			ViewBag.AddUrl = SettingManager.GetValue("ApplicationUrl").ToString().TrimEnd('/') + "/Contact/Add?email=isim@mail.com&firstname=isim&lastname=soyisim";
			ViewBag.DeleteUrl = SettingManager.GetValue("ApplicationUrl").ToString().TrimEnd('/') + "/Contact/Delete?email=isim@mail.com";
			ViewBag.List = _db.Setting.All.Where(x => x.Description.Length > 0).OrderBy(x => x.Description).ToList();
            return View();
        }

		[RequiredAuth]
		public ActionResult SmtpTest()
		{
			var result = EmailManager.SendMail("abuse@google.com", "Test Title", "Test");
			return Redirect("/Setting?smtpTestResult=" + result + "");
		}

		[HttpPost]
		[RequiredAuth]
		public ActionResult Post()
		{
			var settingKeyList = _db.Setting.All.Select(x => x.ItemKey).ToList();
			foreach (var item in settingKeyList)
			{
				_db.Setting.All.Where(x => x.ItemKey == item && x.Description.Length > 0).Update(x => new Setting { ItemValue = Request.Form[item] });
			}
			return Redirect("/Setting?completed=true");
		}

    }
}
