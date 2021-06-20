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

        public int MinPages { get; set; }
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
                try { bot.ClickElement(By.Id("L2AGLb")); } catch { } // "Agree with ToS" button
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
                try { bot.SendKeysToElement(byForSearchAndConfirm, Keys.Enter); } catch { }
            }

            // filter by date
            {
                try
                {
                    bot.ClickElement(By.Id("hdtb-tls")); // "Tools" button
                    Thread.Sleep(250);
                    bot.ClickElement(By.XPath("//*[@class=\"hdtb-mn-hd\"]")); // "Any time" dropdown

                    var timeDropdownItems = bot.driver.FindElements(By.XPath("//*[contains(@class, 'znKVS') and contains(@class, 'tnhqA')]")); // get all items from the time filter dropdown
                    timeDropdownItems.Last().Click(); // click "Custom range..." item in the dropdown
                }
                catch
                {

                }

                try { bot.SendKeysToElement(By.Id("OouJcb"), MinDate); } catch { }
                try { bot.SendKeysToElement(By.Id("rzG2be"), MaxDate); } catch { }
                try { bot.ClickElement(By.XPath("/html/body/div[7]/div/div[4]/div[2]/div[2]/div[3]/form/g-button")); } catch { } // "Go" button
            }

            // read URLs
            {
                for (int i = 0; i < Pages; i++)
                {
                    // reading search results
                    if (i + 1 >= MinPages) // if the page isn't in the specified range, skip it
                    {
                        try
                        {
                            string html = bot.GetElementInnerHTML(By.Id("center_col"));
                            MatchCollection matches = Regex.Matches(html, @"https?:\/\/(www\.)?[-a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b");
                            foreach (Match match in matches)
                            {
                                Websites.Add(new() { Link = match.Value, Page = i + 1 });
                            }
                        }
                        catch
                        {

                        }
                    }

                    // go to next page of search results
                    try
                    {
                        By nextPageButton = By.XPath("//*[contains(@style, 'display:block;margin-left:53px')]");
                        bot.ScrollToElement(nextPageButton);
                        Thread.Sleep(1000);
                        bot.ClickElement(nextPageButton); // go to the next page of search results
                        Thread.Sleep(1000);
                    }
                    catch
                    {
                        break; // no more pages of search results, so continue onto another process
                    }
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
            List<string> links = new List<string>();
            List<int> pages = new List<int>();
            foreach (Website website in Websites)
            {
                bool duplicate = false;
                foreach (string link in links)
                {
                    if (website.Link == link)
                    {
                        duplicate = true;
                        break;
                    }
                }

                if (duplicate) continue;

                links.Add(website.Link);
                pages.Add(website.Page);
            }

            Websites.Clear();
            for (int i = 0; i < links.Count; i++)
            {
                Websites.Add(new() { Link = links[i], Page = pages[i] });
            }
        }

        public void EndProcess()
        {
            bot.CloseDriver();
        }
    }
}
