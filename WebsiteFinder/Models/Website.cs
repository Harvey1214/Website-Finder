using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebsiteFinder
{
    record Website
    {
        public string Link { get; set; } = "";
        public string ConcatenatedEmails
        {
            get
            {
                string emails = "";
                foreach (string email in Emails)
                {
                    emails += $"{email},";
                }

                if (emails.Length == 0) return "";

                return emails.Substring(0, emails.Length - 1);
            }
        }
        public List<string> Emails { get; set; } = new List<string>();
    }
}
