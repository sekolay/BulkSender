using BulkSender.Entity;
using BulkSender.Enum;
using BulkSender.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BulkSender.Controllers
{
    public class PatternController : Base
    {
		[RequiredAuth]
        public ActionResult Index()
        {
			ViewBag.List = _db.Pattern.All.ToList();
            return View();
        }

		[RequiredAuth]
		public ActionResult Post(int? id)
		{
			var viewModel = new Pattern();
			if (id > 0)
			{
				ViewBag.Title = "Şablon Düzenle";
				viewModel = _db.Pattern.GetById(id);
			}
			else
			{
				ViewBag.Title = "Yeni Şablon";
			}
			return View(viewModel);
		}

		[HttpPost]
		[RequiredAuth]
		[ValidateInput(false)]
		public JsonResult Post(int? id, string name, string emailTitle, string emailBody)
		{
			var json = new JsonDataModel();
			if (string.IsNullOrEmpty(name))
			{
				json.result = ResultType.UnSuccess;
				json.message = "Lütfen gerekli alanları doldurunuz";
			}
			else
			{
				if (id > 0)
				{
					var postItem = _db.Pattern.GetById(id);
					postItem.EmailBody = emailBody;
					postItem.EmailTitle = emailTitle;
					postItem.Name = name;
					_db.Pattern.Edit(postItem);
				}
				else
				{
					var postItem = new Pattern();
					postItem.EmailBody = emailBody;
					postItem.EmailTitle = emailTitle;
					postItem.Name = name;
					_db.Pattern.Add(postItem);
				}
				json.result = ResultType.Success;
			}
			return Json(new { json }, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		[RequiredAuth]
		public ResultType Delete(int id)
		{
			_db.Pattern.Delete(x => x.Id == id);
			_db.History.Delete(x => x.PatternId == id);
			return ResultType.Success;
		}

    }
}
