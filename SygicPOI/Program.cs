using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using CsvHelper;

namespace SygicPOI
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("SygicPOI Converter for CAMC POIs");
            Console.WriteLine("================================");

            var sygicSites = new List<SygicFormat>();

            var regex = new Regex("\"(?<Name>[^>]*)>(?<Phone>\\d+\\s?\\d+)?[\\s,]*(?<Address>[^\"]*)\"", RegexOptions.Singleline);

            using (var reader = new StreamReader(@"C:\Users\barri\Downloads\cclubclsgarminnavman.csv"))
            {
                using (var csv = new CsvReader(reader))
                {
                    csv.Configuration.HasHeaderRecord = false;

                    var camcSites = csv.GetRecords<CAMCFormat>();

                    foreach (var camcSite in camcSites)
                    {
                        var match = regex.Match(camcSite.TheRest);

                        if (match.Success)
                        {
                            sygicSites.Add(new SygicFormat
                            {
                                Longitude = camcSite.Longitude,
                                Latitude = camcSite.Latitude,
                                Name = match.Groups["Name"].Value,
                                Phone = match.Groups["Phone"].Value,
                                Address = match.Groups["Address"].Value,
                            });
                        }
                        else
                        {
                            Console.WriteLine($"Failed to match site: [{camcSite.TheRest}]");
                        }
                    }
                }
            }

            using (var writer = new StreamWriter(@"C:\Users\barri\Downloads\camc_sygic.csv"))
            {
                using (var csv = new CsvWriter(writer))
                {
                    csv.Configuration.HasHeaderRecord = false;

                    csv.WriteRecords(sygicSites);
                }
            }

            Console.ReadLine();
        }
    }

    public class CAMCFormat
    {
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public string TheRest { get; set; }
    }

    public class SygicFormat
    {
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Web { get; set; }
        public string Email { get; set; }
    }
}
