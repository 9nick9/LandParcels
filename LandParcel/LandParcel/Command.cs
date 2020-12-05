using System;
using System.Collections.Generic;
using System.Text;

namespace LandParcel
{
	public class Command
	{
		public CommandModes Mode { get; set; }
		public string CompanyId { get; set; }
		public Dictionary<string, Company> Companies { get; set; }

		public void PerformAction()
		{
			switch (Mode)
			{
				case CommandModes.Exit:
					Environment.Exit(1); 
					break;
				case CommandModes.Root:
					OutputRootData(CompanyId);
					break;
				case CommandModes.Expand:
					OutputExpandData(CompanyId, 0);
					break;
				case CommandModes.Help:
				default:
					OutputHelpData();
					break;
			}
		}

		private void OutputExpandData(string companyId, int level)
		{
			var company = Companies[companyId];
			int parcels = GetParcels(companyId);

			PrintCompany(company, parcels, level);

			foreach(var child in company.Subsidiaries)
			{
				OutputExpandData(child, level + 1);
			}
		}

		void PrintCompany(Company company, int parcels, int level, bool isTarget = false)
		{
			for (int i = 0; i < level; i++)
				Console.Write("|");

			string targetId = isTarget ? " ***" : string.Empty;

			Console.WriteLine($"{company.Id}; {company.Name}; owner of {parcels} land parcels{targetId}");
		}

		private void OutputRootData(string companyId)
		{
			var lookingAt = companyId;
			int levelsDeep = 0;
			while(!string.IsNullOrEmpty(Companies[lookingAt].ParentId))
			{
				levelsDeep++;
				lookingAt = Companies[lookingAt].ParentId;
			}

			PrintFromRoot(lookingAt, 0, levelsDeep, companyId);
		}

		void PrintFromRoot(string companyId, int level, int maxLevels, string target)
		{
			PrintCompany(Companies[companyId], GetParcels(companyId), level, companyId == target);
			if (level < maxLevels - 1 || (level == maxLevels -1 && Companies[target].ParentId == companyId))
			{
				foreach(var child in Companies[companyId].Subsidiaries)
				{
					PrintFromRoot(child, level + 1, maxLevels, target);
				}
			}
		}

		int GetParcels(string companyId)
		{
			var company = Companies[companyId];
			int parcels = company.Parcels;
			foreach (var child in company.Subsidiaries)
			{
				parcels += GetParcels(child);
			}

			return parcels;
		}

		private void OutputHelpData()
		{
			Console.Out.WriteLine("The following commands are allowed:");
			Console.Out.WriteLine("from_root {company_id}");
			Console.Out.WriteLine("expand {company_id}");
			Console.Out.WriteLine();
			Console.Out.WriteLine("from_root will show the number of parcels owned by the company of the subsidiaries at that level and all higher levels on that branch of the tree.");
			Console.Out.WriteLine();
			Console.Out.WriteLine("expand will fully expand the tree of subsidiaries and the number of parcels owned by each.");
		}
	}

	public enum CommandModes
	{
		Root,
		Expand,
		Exit,
		Help
	}
}
