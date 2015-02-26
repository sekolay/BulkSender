using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BulkSender.Repository;
using BulkSender.Common;

namespace BulkSender.Controllers
{
	public class Base : Controller
	{
		public readonly UnitOfWork _db = new UnitOfWork();

		public class RequiredAuthAttribute : ActionFilterAttribute, IActionFilter
		{
			void IActionFilter.OnActionExecuting(ActionExecutingContext filterContext)
			{
				string returnUrl = "~/Login?returnUrl=" + System.Web.HttpContext.Current.Server.UrlEncode(System.Web.HttpContext.Current.Request.ServerVariables["URL"] + "?" + System.Web.HttpContext.Current.Request.ServerVariables["QUERY_STRING"]) + "";
				if (SessionManager.CurrentSession == null)
				{
					filterContext.Result = new RedirectResult(returnUrl);
				}
			}

		}
	}
}