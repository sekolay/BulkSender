using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BulkSender.Enum;

namespace BulkSender.Model
{
	/// <summary>
	/// Calls for service that converts objects to JSON format default model
	/// </summary>
	[Serializable]
	public partial class JsonDataModel
	{
		//public int iDisplayLength { get; set; }
		//public int iTotalDisplayRecords { get; set; }
		public ResultType result { get; set; }
		public string message { get; set; }
		public object aaData { get; set; }
	}
}