using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BulkSender.Entity;
using BulkSender.Repository;

namespace BulkSender.Common
{
	public class SettingManager
	{
		private static readonly object _locker = new object();

		public static object GetValue(string itemKey)
		{
			var getSetting = CachedSettingList().Where(x => x.ItemKey == itemKey).FirstOrDefault();
			if (getSetting != null && getSetting.ItemValue == null)
				return null;
			if (getSetting.ItemValue.GetType() == typeof(DBNull))
				return null;
			return Convert.ChangeType(getSetting.ItemValue, Type.GetType(getSetting.ObjectType));
		}

		public static List<Setting> CachedSettingList()
		{
			var settingList = (List<Setting>)CacheManager.GetCache(CacheManager.SettingListKey);
			if (settingList == null)
			{
				lock (_locker)
				{
					var db = new UnitOfWork();
					settingList = db.Setting.All.ToList();
					CacheManager.SetCache(CacheManager.SettingListKey, settingList);
				}
			}
			return settingList;
		}

	}
}