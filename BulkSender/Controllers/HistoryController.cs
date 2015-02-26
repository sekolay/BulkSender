using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BulkSender.Controllers
{
    public class HistoryController : Base
    {
		[RequiredAuth]
        public ActionResult Index()
        {
			ViewBag.List = _db.History.All.ToList();
			ViewBag.GroupList = _db.Group.All.ToList();
            return View();
        }

    }
}
