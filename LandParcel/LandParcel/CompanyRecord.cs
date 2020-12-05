using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace LandParcel
{
	public class CompanyRecord
	{
		[Name("company_id")]
		public string CompanyId { get; set; }
		[Name("name")]
		public string Name { get; set; }
		[Name("parent")]
		public string ParentId { get; set; }
	}


}
