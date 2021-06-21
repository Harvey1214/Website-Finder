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
        public bool FilterByFooterDate { get; init; }
        public string MinDate { get; init; }
        public string MaxDate { get; init; }

        public List<Website> GetEmails(List<Website> websites)
        {
            List<Website> results = new();

            foreach (Website website in websites)
            {
                string html = "";
                using MyWebClient client = new();

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

                    // finding footer date
                    {
                        string footerDate = Regex.Match(html, @"([©®™℠] ?\d\d\d\d)|(\d\d\d\d ?[©®™℠])").Value;

                        if (footerDate.Length == 0 || footerDate == "0")
                        {
                            MatchCollection matches = Regex.Matches(html, @"\d\d\d\d");
                            foreach (Match match in matches)
                            {
                                int year = Int32.Parse(match.Value);
                                if (year > 1970 && year < DateTime.Now.Year + 5)
                                {
                                    website.FooterDate = year;
                                    break;
                                }
                            } 
                        }
                        else
                        {
                            int date = 0;
                            bool success = Int32.TryParse(Regex.Match(footerDate, @"\d\d\d\d").Value, out date);

                            if (success) website.FooterDate = date;
                        }
                    }

                    if (FilterByFooterDate)
                    {
                        int minDate = Convert.ToInt32(MinDate);
                        int maxDate = Convert.ToInt32(MaxDate);
                        if (website.FooterDate < minDate && minDate != 0) continue;
                        if (website.FooterDate > maxDate && maxDate != 0) continue;
                    }

                    results.Add(website);
                }
                catch
                {

                }
            }

            return results;
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
