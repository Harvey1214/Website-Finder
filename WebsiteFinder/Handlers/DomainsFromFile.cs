using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace WebsiteFinder
{
    static class DomainsFromFile
    {
        public static List<Website> Get(string path)
        {
            List<string> lines = File.ReadAllLines(path).ToList();

            List<Website> results = new();

            foreach (string line in lines)
            {
                string websiteURL = $"http://www.{line.Split('@').LastOrDefault()}";

                if (websiteURL == null) continue;
                if (websiteURL.Length == 0) continue;

                results.Add(new() { Link = websiteURL });
            }

            return results;
        }
    }
}
