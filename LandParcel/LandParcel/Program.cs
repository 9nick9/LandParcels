using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace LandParcel
{
	class Program
	{
        static Dictionary<string, Company> companies = new Dictionary<string, Company>();
        static async Task Main(string[] args)
		{
            System.Console.WriteLine("Loading Data...");
            await LoadData();

            string input;
            do
            {
                System.Console.WriteLine("Please enter command or enter help for a list of commands.");
                input = System.Console.ReadLine() ?? "";
                var enteredText = input.ToLower();

                try
                {
                    Command command = ParseCommand(enteredText);
                    command.PerformAction();
                }
                catch (ArgumentException e)
                {
                    System.Console.ForegroundColor = ConsoleColor.Red;
                    System.Console.WriteLine(e.Message);
                    System.Console.ResetColor();
                }
            } while (input != "exit");
        }

		private static Command ParseCommand(string text)
		{
            var split = text.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            var result = new Command
            {
                Mode = CommandModes.Help,
                Companies = companies
            };

            if (split.Length > 2 || split.Length < 1 || (split.Length == 1 && split[0] != "exit"))
            {
                return result;
            }

            switch (split[0])
            {
                case "expand":
                    result.Mode = CommandModes.Expand;
                    result.CompanyId = split[1];
                    break;
                case "from_root":
                    result.Mode = CommandModes.Root;
                    result.CompanyId = split[1];
                    break;
                case "exit":
                    result.Mode = CommandModes.Exit;
                    break;
			}

            if (result.CompanyId != null && !result.Companies.ContainsKey(result.CompanyId))
                throw new ArgumentException($"The company Id {result.CompanyId} does not exist.");
            return result;
		}

		private static async Task LoadData()
		{
            using (HttpClient client = new HttpClient())
			{
                var response = await client.GetAsync("https://raw.githubusercontent.com/landtechnologies/technical-challenge/master/land-ownership/company_relations.csv");
                var companiesStream = await response.Content.ReadAsStreamAsync();

                IEnumerable<CompanyRecord> companyRecords;
                using (var reader = new StreamReader(companiesStream))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    companyRecords = csv.GetRecords<CompanyRecord>().ToList();
                }

                var ownResponse = await client.GetAsync("https://raw.githubusercontent.com/landtechnologies/technical-challenge/master/land-ownership/land_ownership.csv");
                var ownStream = await ownResponse.Content.ReadAsStreamAsync();

                IEnumerable<OwnershipRecord> ownerships;
                using (var reader = new StreamReader(ownStream))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    ownerships = csv.GetRecords<OwnershipRecord>().ToList();
                }

                foreach(var company in companyRecords)
				{
                    companies.Add(company.CompanyId.ToLower(), new Company
                    {
                        Id = company.CompanyId.ToLower(),
                        Parcels = 0,
                        ParentId = string.IsNullOrEmpty(company.ParentId) ? null : company.ParentId.ToLower(),
                        Subsidiaries = new HashSet<string>(),
                        Name = company.Name
                    });
				}

                foreach (var company in companyRecords)
                {
                    if (string.IsNullOrEmpty(company.ParentId))
                        continue;

                    companies[company.ParentId.ToLower()].Subsidiaries.Add(company.CompanyId.ToLower());                    
                }

                foreach(var ownership in ownerships)
				{
                    companies[ownership.CompanyId.ToLower()].Parcels++;
                }
            }
		}
	}
}
 