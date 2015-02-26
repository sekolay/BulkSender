using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BulkSender.Common;
using BulkSender.Model;

namespace BulkSender.Controllers
{
	public class LoginController : Base
	{
		public ActionResult Index()
		{
			return View();
		}

		[HttpPost]
		public ActionResult Index(string username, string password, bool? isRemember)
		{
			var getItem = _db.Setting.All.Count(x => (x.ItemKey == "LoginName" && x.ItemValue == username) || (x.ItemKey == "LoginPassword" && x.ItemValue == password));
			if (getItem == 2)
			{
				var newSession = new SessionIdentity();
				newSession.Username = username;
				// Creating Session
				SessionManager.CurrentSession = newSession;
				if (isRemember == true)
				{
					Response.Cookies["HPUsername"].Expires = DateTime.UtcNow.AddDays(30);
					Response.Cookies["HPPassword"].Expires = DateTime.UtcNow.AddDays(30);
					Response.Cookies["HPUsername"].Value = UtilManager.ConvertToBase64(username);
					Response.Cookies["HPUsername"].Path = "/";
					Response.Cookies["HPPassword"].Value = UtilManager.ConvertToBase64(password);
					Response.Cookies["HPPassword"].Path = "/";
				}
				return Redirect("/Group");
			}
			ModelState.AddModelError("Username", "Bilgiler Doğrulanamadı");
			return View();
		}

		public ActionResult Off()
		{
			Session.Abandon();
			return Redirect("/Login");
		}

	}
}
