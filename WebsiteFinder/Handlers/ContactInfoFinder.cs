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
        public List<Website> GetEmails(List<Website> websites)
        {
            foreach (Website website in websites)
            {
                string html = "";
                using (MyWebClient client = new())
                {
                    try
                    {
                        html = client.DownloadString(website.Link);
                        try { html += client.DownloadString($"{website.Link}/contact"); } catch { }
                        try { html += client.DownloadString($"{website.Link}/support"); } catch { }

                        // finding a contact email using regex
                        {
                            MatchCollection matches = Regex.Matches(html, "[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\\.[A-Za-z]{2,4}");
                            foreach (Match match in matches)
                            {
                                website.Emails.Add(match.Value);
                            }

                            website.Emails = RemoveDuplicates(website.Emails);
                        }
                    }
                    catch
                    {

                    }
                }
            }

            return websites;
        }

        private List<string> RemoveDuplicates(List<string> list)
        {
            List<string> located = new List<string>();

            foreach (string text in list)
            {
                bool duplicate = false;
                foreach (string locatedText in located)
                {
                    if (locatedText == text)
                    {
                        duplicate = true;
                    }
                }

                if (duplicate == false && Regex.IsMatch(text, "\\.?((png)|(jpg)|(webp))|(svg)|(ico)") == false) located.Add(text);
            }

            return located;
        }
    }

    class MyWebClient : WebClient
    {
        protected override WebRequest GetWebRequest(Uri uri)
        {
            WebRequest w = base.GetWebRequest(uri);
            w.Timeout = (int)TimeSpan.FromSeconds(5).TotalMilliseconds;
            return w;
        }
    }
}
