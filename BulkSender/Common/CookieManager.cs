using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BulkSender.Common
{
	public class CookieManager
	{
		private static readonly object _locker = new object();

		/// <summary> 
		/// Get
		/// </summary>
		/// <param name="key"></param>
		public static string Get(string key)
		{
			var cookie = HttpContext.Current.Request.Cookies[key];
			if (cookie != null)
				return cookie.Value.Replace("" + key + "=", "");
			else
				return string.Empty;
		}

		/// <summary>
		/// Remove
		/// </summary>
		/// <param name="key"></param>
		public static void Remove(string key)
		{
			Set(key, "", -999999);
		}

		/// <summary>
		/// Set
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <param name="expiryTimeMinutes"></param>
		public static void Set(string key, string value, int expiryTimeMinutes = 0)
		{
			lock (_locker)
			{
				HttpCookie httpCookie = new HttpCookie(key);
				httpCookie.Path = "/";
				httpCookie.Value = value;
				if (expiryTimeMinutes != 0)
				{
					httpCookie.Expires = DateTime.Now.AddMinutes(expiryTimeMinutes);
				}
				HttpContext.Current.Response.Cookies.Add(httpCookie);
			}
		}
	}
}