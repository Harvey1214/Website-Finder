using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using System.Threading;
using System.Text.RegularExpressions;

namespace WebsiteFinder
{
    class ActionsManager
    {
        private Bot bot = new Bot();

        public string KeyWords { get; set; } = "Hi there it works!";
        public string MinDate { get; set; } = "2000";
        public string MaxDate { get; set; } = "2005";

        public List<Website> Websites { get; set; } = new List<Website>();

        public int Pages { get; set; } = 4;

        public void StartProcess(bool maximized = true, bool headless = false)
        {
            ScrapeSearchResults(maximized, headless);
        }

        private void ScrapeSearchResults(bool maximized, bool headless)
        {
            bot.OpenChrome(maximized, headless);

            // search using keywords
            {
                bot.GoToURL("https://www.google.com");
                bot.ClickElement(By.Id("L2AGLb")); // "Agree with ToS" button
                // type keywords
                By byForSearchAndConfirm;
                try
                {
                    byForSearchAndConfirm = By.XPath("//input[@placeholder='Search Google or type a URL']");
                    bot.SendKeysToElement(byForSearchAndConfirm, KeyWords);
                }
                catch
                {
                    try
                    {
                        byForSearchAndConfirm = By.XPath("//input[@type='text']");
                        bot.SendKeysToElement(byForSearchAndConfirm, KeyWords);
                    }
                    catch
                    {
                        try
                        {
                            byForSearchAndConfirm = By.XPath("//input[@type='search']");
                            bot.SendKeysToElement(byForSearchAndConfirm, KeyWords);
                        }
                        catch
                        {
                            byForSearchAndConfirm = By.ClassName("gLFyf");
                            bot.SendKeysToElement(byForSearchAndConfirm, KeyWords);
                        }
                    }
                }
                // confirm
                bot.SendKeysToElement(byForSearchAndConfirm, Keys.Enter);
            }

            // filter by date
            {
                bot.ClickElement(By.Id("hdtb-tls")); // "Tools" button
                Thread.Sleep(250);
                bot.ClickElement(By.XPath("//*[@class=\"hdtb-mn-hd\"]")); // "Any time" dropdown

                var timeDropdownItems = bot.driver.FindElements(By.XPath("//*[contains(@class, 'znKVS') and contains(@class, 'tnhqA')]")); // get all items from the time filter dropdown
                timeDropdownItems.Last().Click(); // click "Custom range..." item in the dropdown

                bot.SendKeysToElement(By.Id("OouJcb"), MinDate);
                bot.SendKeysToElement(By.Id("rzG2be"), MaxDate);
                bot.ClickElement(By.XPath("/html/body/div[7]/div/div[4]/div[2]/div[2]/div[3]/form/g-button")); // "Go" button
            }

            // read URLs
            {
                for (int i = 0; i < Pages; i++)
                {
                    string html = bot.GetElementInnerHTML(By.Id("center_col"));
                    MatchCollection matches = Regex.Matches(html, @"https?:\/\/(www\.)?[-a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b");
                    foreach (Match match in matches)
                    {
                        Websites.Add(new() { Link = match.Value });
                    }

                    By nextPageButton = By.XPath("//*[contains(@style, 'display:block;margin-left:53px')]");
                    bot.ScrollToElement(nextPageButton);
                    Thread.Sleep(1000);
                    bot.ClickElement(nextPageButton); // go to the next page of search results
                    Thread.Sleep(1000);
                }

                RemoveDuplicateWebsites();
            }

            // driver action ends
            {
                EndProcess();
            }
        }

        private void RemoveDuplicateWebsites()
        {
            List<Website> results = new();
            foreach (Website website in Websites)
            {
                if (results.Contains(website) == false)
                {
                    results.Add(website);
                }
            }

            Websites = results;
            Websites.RemoveAll(o => o.Link.Length == 0);
        }

        public void EndProcess()
        {
            bot.CloseDriver();
        }
    }
}
