using BulkSender.Common;
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
    public class ContactController : Base
    {
		[RequiredAuth]
        public ActionResult Index(int groupId)
		{
			var getGroup = _db.Group.All.Where(x => x.Id == groupId).FirstOrDefault();
			if (getGroup == null)
				return Redirect("/Group");
			ViewBag.GroupName = getGroup.Name;
			ViewBag.GroupId = getGroup.Id;
			ViewBag.List = _db.Contact.All.Where(x => x.GroupId == groupId).ToList();
			return View();
        }

		[RequiredAuth]
		public ActionResult Post(long id, int groupId)
		{
			var getGroup = _db.Group.All.Where(x => x.Id == groupId).FirstOrDefault();
			if (getGroup == null)
				return Redirect("/Group");
			ViewBag.GroupName = getGroup.Name;
			var viewModel = new Contact();
			if (id > 0)
			{
				ViewBag.Title = "Kişi Düzenle";
				viewModel = _db.Contact.GetById(id);
			}
			else
			{
				viewModel.GroupId = groupId;
				ViewBag.Title = "Yeni Kişi";
			}
			return View(viewModel);
		}

		[HttpPost]
		[RequiredAuth]
		public JsonResult Post(long? id, int groupId, string email, string firstname, string lastname)
		{
			var json = new JsonDataModel();
			if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(firstname) || string.IsNullOrEmpty(lastname))
			{
				json.result = ResultType.UnSuccess;
				json.message = "Lütfen tüm alanları doldurunuz";
			}
			else
			{
				if (id > 0)
				{
					var postItem = _db.Contact.GetById(id);
					postItem.GroupId = groupId;
					postItem.Email = email;
					postItem.FirstName = firstname;
					postItem.LastName = lastname;
					_db.Contact.Edit(postItem);
				}
				else
				{
					var isExistContact = _db.Contact.All.Any(x => x.Email == email.ToLower() && x.GroupId == groupId);
					if (isExistContact == false && email.Length > 0 && UtilManager.EmailIsValid(email))
					{
						var postItem = new Contact();
						postItem.Email = email.ToLower();
						postItem.GroupId = groupId;
						postItem.FirstName = firstname;
						postItem.LastName = lastname;
						postItem.CreateDate = DateTime.Now;
						_db.Contact.Add(postItem);
						json.result = ResultType.Success;
					}
					else {
						json.result = ResultType.UnSuccess;
						json.message = "Bu mail adresi grupta bulunuyor";
					}

				}
				
			}
			return Json(new { json }, JsonRequestBehavior.AllowGet);
		}

		// PUBLIC METHOD
		public ResultType Add(string email, string firstname, string lastname)
		{
			var groupId = _db.Group.All.OrderBy(x => x.Id).FirstOrDefault().Id;
			var isExistContact = _db.Contact.All.Any(x => x.Email == email.ToLower() && x.GroupId == groupId);
			if (isExistContact == false && email.Length > 0 && UtilManager.EmailIsValid(email))
			{
				var postItem = new Contact();
				postItem.Email = email.ToLower();
				postItem.GroupId = groupId;
				postItem.FirstName = firstname;
				postItem.LastName = lastname;
				postItem.CreateDate = DateTime.Now;
				_db.Contact.Add(postItem);
				return ResultType.Success;
			}
			else
			{
				return ResultType.UnSuccess;
			}
		}

		// PUBLIC METHOD
		public ResultType Delete(string email)
		{
			_db.Contact.Delete(x => x.Email == email);
			return ResultType.Success;
		}

		[RequiredAuth]
		public ResultType DeleteFromId(long id)
		{
			_db.Contact.Delete(x => x.Id == id);
			return ResultType.Success;
		}


    }
}
