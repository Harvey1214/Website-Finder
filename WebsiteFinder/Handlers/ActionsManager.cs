using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using System.Threading;

namespace WebsiteFinder
{
    class ActionsManager
    {
        private Bot bot = new Bot();

        public string KeyWords { get; set; } = "Hi there it works!";
        public string MinDate { get; set; } = "2000";
        public string MaxDate { get; set; } = "2005";

        public void StartProcess(bool maximized = true, bool headless = false)
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

            }
        }

        public void EndProcess()
        {
            bot.CloseDriver();
        }
    }
}
