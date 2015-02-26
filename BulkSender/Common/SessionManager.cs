using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BulkSender.Model;

namespace BulkSender.Common
{
	public static class SessionManager
	{
		private const string CurrentSessionKey = "CurrentSessionKey";
		private const int SessionTimeout = 2000;

		public static SessionIdentity CurrentSession
		{
			get
			{
				var context = HttpContext.Current;
				if (context == null) return null;
				if (context.Session == null) return null;
				var sessionIdentity = context.Session[CurrentSessionKey] as SessionIdentity;
				return sessionIdentity;
			}
			set
			{
				HttpContext.Current.Session.Timeout = SessionTimeout;
				HttpContext.Current.Session[CurrentSessionKey] = value;
			}
		}

	}
}