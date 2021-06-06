using System;
using System.Collections.Generic;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace WebsiteFinder
{
    public class Bot
    {
        [ThreadStatic]
        public IWebDriver driver;

        private bool isOpen = false;

        public void OpenChrome(bool maximized = true, bool headless = false)
        {
            isOpen = true;

            ChromeOptions options = new ChromeOptions();

            //options.AddExtension(@"anticaptcha-plugin.crx");
            if (maximized)
            {
                options.AddArgument("--start-maximized");
            }
            if (headless)
            {
                options.AddArgument("--headless");
            }

            driver = new ChromeDriver(options);
        }

        public void CloseDriver()
        {
            if (isOpen)
            {
                isOpen = false;
                try
                {
                    driver.Close();
                }
                catch
                {

                }
            }
        }

        public string GetCurrentURL()
        {
            return driver.Url;
        }

        public string GetElementInnerHTML(By by)
        {
            return driver.FindElement(by).GetAttribute("innerHTML");
        }

        public void GoToURL(string url)
        {
            driver.Navigate().GoToUrl(url);
        }

        public void ClickElement(By by)
        {
            driver.FindElement(by).Click();
        }

        public void SendKeysToElement(By by, string content)
        {
            driver.FindElement(by).SendKeys(content);
        }

        public void SwitchIFrame(string frameName)
        {
            driver.SwitchTo().Frame(frameName);
        }

        public void ScrollToElement(By by)
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;

            //Find element by link text and store in variable "Element"        		
            IWebElement Element = driver.FindElement(by);

            //This will scroll the page till the element is found		
            js.ExecuteScript("arguments[0].scrollIntoView();", Element);
        }
    }
}
