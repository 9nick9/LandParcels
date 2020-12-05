using System;
using System.Collections.Generic;
using System.Text;

namespace LandParcel
{
	public class Company
	{
		public string Id { get; set; }
		public int Parcels { get; set; }
		public string ParentId { get; set; }
		public HashSet<string> Subsidiaries { get; set; }
		public string Name { get; set; }
	}
}
