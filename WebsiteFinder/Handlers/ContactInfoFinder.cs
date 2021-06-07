using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WebsiteFinder
{
    class ContactInfoFinder
    {
        public List<Website> FindEmails(List<Website> websites)
        {
            foreach (Website website in websites)
            {
                string html;
                using (WebClient client = new WebClient())
                {
                    html = client.DownloadString(website.Link);
                }

                // fix the regular expression
                MatchCollection matches = Regex.Matches(html, "(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|\"(?:[\x01-\x08\x0b\x0c\x0e -\x1f\x21\x23 -\x5b\x5d -\x7f]|\\[\x01-\x09\x0b\x0c\x0e -\x7f])*\")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])");
                foreach (Match match in matches)
                {
                    website.Emails.Add(match.Value);
                }
            }

            return websites;
        }
    }
}
