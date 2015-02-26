using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BulkSender.Model
{
	public class FileModel
	{
		public string Name { get; set; }
		public long SizeKb { get; set; }
		public string Resolution { get; set; }
		public DateTime CreateDate { get; set; }
	}
}