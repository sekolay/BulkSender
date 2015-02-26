using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BulkSender.Entity;
using BulkSender.Enum;
using BulkSender.Model;
using BulkSender.Common;

namespace BulkSender.Controllers
{
	public class GroupController : Base
	{
		[RequiredAuth]
		public ActionResult Index()
		{
			ViewBag.List = (from g in _db.Group.All
								 join ct in _db.Contact.All on g.Id equals ct.GroupId into ACS
								 from acs in ACS.DefaultIfEmpty()
								 select new
								 {
									 g.Description,
									 g.Id,
									 g.LastSentDate,
									 g.Name,
									 ContactTotal = ACS.Count(x => x.GroupId == g.Id)
								 }).Distinct().ToList().ToNonAnonymousList(typeof(Group)) as List<Group>;
			return View();
		}

		[RequiredAuth]
		public ActionResult Post(int? id)
		{
			var viewModel = new Group();
			if (id > 0)
			{
				ViewBag.Title = "Grup Düzenle";
				viewModel = _db.Group.GetById(id);
			}
			else
			{
				ViewBag.Title = "Yeni Grup";
			}
			return View(viewModel);
		}

		[HttpPost]
		[RequiredAuth]
		public JsonResult Post(int? id, string name, string description)
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
					var postItem = _db.Group.GetById(id);
					postItem.Description = description;
					postItem.Name = name;
					_db.Group.Edit(postItem);
				}
				else
				{
					var postItem = new Group();
					postItem.Description = description;
					postItem.Name = name;
					_db.Group.Add(postItem);
				}
				json.result = ResultType.Success;
			}
			return Json(new { json }, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		[RequiredAuth]
		public ResultType Delete(int id)
		{
			_db.Group.Delete(x => x.Id == id);
			_db.Contact.Delete(x => x.GroupId == id);
			_db.History.Delete(x => x.GroupIdList.Contains("," + id + ","));
			return ResultType.Success;
		}

	}
}
