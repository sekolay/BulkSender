using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;

namespace BulkSender.Common
{
	public static class CacheManager
	{
		public const int DefaultCacheMinutes = 240;
		public const string SettingListKey = "SettingList";

		private static readonly object _locker = new object();

		public static object GetCache(string cacheKey)
		{
			var objCache = HttpRuntime.Cache;
			return objCache.Get(cacheKey);
		}

		public static void SetCache(string cacheKey, object objObject)
		{
			SetCache(cacheKey, objObject, null);
		}

		public static void SetCache(string cacheKey, object objObject, CacheDependency objDependency)
		{
			lock (_locker)
			{
				var objCache = HttpRuntime.Cache;
				objCache.Insert(cacheKey, objObject, objDependency, DateTime.Now.AddMinutes(DefaultCacheMinutes), Cache.NoSlidingExpiration);
			}
		}

		public static void SetCache(string cacheKey, object objObject, int expirationMinute)
		{
			lock (_locker)
			{
				var objCache = HttpRuntime.Cache;
				objCache.Insert(cacheKey, objObject, null, DateTime.Now.AddMinutes(expirationMinute), Cache.NoSlidingExpiration);
			}
		}

		public static void RemoveCache(string cacheKey)
		{
			lock (_locker)
			{
				if (GetCache(cacheKey) == null) return;
				var objCache = HttpRuntime.Cache;
				if (objCache.Get(cacheKey) != null)
					objCache.Remove(cacheKey);
			}
		}

	}
}