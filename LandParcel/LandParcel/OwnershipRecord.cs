using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace LandParcel
{
	public class OwnershipRecord
	{
		[Name("land_id")]
		public string LandId { get; set; }
		[Name("company_id")]
		public string CompanyId { get; set; }
	}
}
