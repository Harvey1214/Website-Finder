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

            // finding websites that contain usual email providers
            List<Website> websitesToRemove = new();
            foreach (Website website in results)
            {
                string link = website.Link.ToLower();
                if (link.Contains("yahoo") || link.Contains("gmail") || link.Contains("outlook") || link.Contains("aol"))
                {
                    websitesToRemove.Add(website);
                }
            }

            // removing websites that have a usual email provider
            foreach (Website websiteToRemove in websitesToRemove)
            {
                results.Remove(websiteToRemove);
            }

            return results;
        }
    }
}
