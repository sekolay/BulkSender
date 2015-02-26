using BulkSender.Common;
using BulkSender.Entity;
using BulkSender.Enum;
using EntityFramework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BulkSender.Controllers
{
    public class SenderController : Base
    {
		[RequiredAuth]
        public ActionResult Index()
        {
			ViewBag.GroupList = (from g in _db.Group.All
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
			ViewBag.PatternList = _db.Pattern.All.ToList();
            return View();
        }

		[RequiredAuth]
		[ValidateInput(false)]
		public ActionResult Post(string emailHeader, string emailFooter, int patternId, int[] groupList)
		{
			string groupIdList = string.Empty;
			var totalSuccess = 0;
			var totalError = 0;
			var receiverList = (from ct in _db.Contact.All
								where groupList.Contains((int)ct.GroupId)
								select ct).GroupBy(x => x.Email).Select(x => x.FirstOrDefault()).ToList();
			var getPattern = _db.Pattern.All.FirstOrDefault(x => x.Id == patternId);
			Parallel.ForEach(receiverList, item =>{
				var body = emailHeader + getPattern.EmailBody + emailFooter;
				var title = getPattern.EmailTitle;
				// Definitions
				body = body.Replace("{email}", item.Email).Replace("{firstName}", item.FirstName).Replace("{lastName}", item.LastName).Replace("{patternId}", patternId.ToString());
				title = title.Replace("{email}", item.Email).Replace("{firstName}", item.FirstName).Replace("{lastName}", item.LastName).Replace("{appUrl}", SettingManager.GetValue("ApplicationUrl").ToString());
				var deliveryStatus = EmailManager.SendMail(item.Email, title, body);
				if (deliveryStatus == ResultType.Success)
					totalSuccess += 1;
				else
					totalError += 1;
				if (groupIdList.Contains("," + item.GroupId + ",") == false) {
					groupIdList += "," + item.GroupId + ",";
				}
			});
			var newHistory = new History();
			newHistory.CreateDate = DateTime.Now;
			newHistory.EmailBody = getPattern.EmailBody;
			newHistory.EmailTitle = getPattern.EmailTitle;
			newHistory.GroupIdList = groupIdList.Replace(",,", ",");
			_db.History.Add(newHistory);
			_db.Setting.All.Where(x => x.ItemKey == "EmailHeader").Update(x => new Setting { ItemValue = emailHeader });
			_db.Setting.All.Where(x => x.ItemKey == "EmailFooter").Update(x => new Setting { ItemValue = emailFooter });
			_db.Pattern.All.Where(x => x.Id == getPattern.Id).Update(x => new Pattern { LastSentDate = DateTime.Now });
			return Redirect("/Sender?completed=true&totalSuccess=" + totalSuccess + "&totalError=" + totalError + "");
		}

		public string GetEmailContent(int patternId, string firstName, string lastName, string email)
		{
			var getPattern = _db.Pattern.All.FirstOrDefault(x => x.Id == patternId);
			var html = "<html><title>" + getPattern.EmailTitle + "</title>" + getPattern.EmailBody + "</html>";
			return html;
		}

    }
}
