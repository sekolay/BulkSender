using BulkSender.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Text;

namespace BulkSender.Common
{
	public class EmailManager
	{

		public static ResultType SendMail(string receiverEmail, string mailTitle, string mailBody)
		{
			var senderName = SettingManager.GetValue("SenderFromName");
			var senderEmail = SettingManager.GetValue("SenderFromEmail");
			var message = new MailMessage("" + senderName + " <" + senderEmail + ">", receiverEmail, mailTitle, mailBody);
			message.IsBodyHtml = true;
			var result = SmtpEmailSender(message);
			return result;
		}

		private static ResultType SmtpEmailSender(MailMessage mailMessage)
		{
			try
			{
				SmtpClient client = new SmtpClient();
				client.Port = Convert.ToInt32(SettingManager.GetValue("SmtpPort").ToString());
				client.Host = SettingManager.GetValue("SmtpServer").ToString();
				client.Timeout = 10000;
				client.DeliveryMethod = SmtpDeliveryMethod.Network;
				client.UseDefaultCredentials = false;
				client.Credentials = new System.Net.NetworkCredential(SettingManager.GetValue("SmtpUsername").ToString(), SettingManager.GetValue("SmtpPassword").ToString());
				client.Send(mailMessage);
				return ResultType.Success;
			}
			catch(Exception ex)
			{
				var exception = ex;
				return ResultType.UnSuccess;
			}
		}
	}
}