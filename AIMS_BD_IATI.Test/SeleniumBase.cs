using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

using OpenQA.Selenium.Firefox;
using TechTalk.SpecFlow;

namespace AIMS_BD_IATI.Test
{
    public class SeleniumBase
    {
        public RemoteWebDriver driver;
        public StringBuilder verificationErrors;
        //private string baseURL;
        public bool acceptNextAlert = true;

        public IOptions opt;
        public ITimeouts timeouts;
        public WebDriverWait wait;

        //public TestContext TestContext { get; set; }

        [Before]
        public void SetupTest()
        {
            //baseURL = "http://localhost:53013/";

            driver = new ChromeDriver();
            //d.Manage().Window.Maximize();
            opt = driver.Manage();
            timeouts = opt.Timeouts();
            timeouts.ImplicitWait = TimeSpan.FromSeconds(10);

            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        [After]
        public void TeardownTest()
        {
            try
            {
                driver.Quit();
            }
            catch (Exception)
            {
                // Ignore errors if unable to close the browser
            }
        }


        private bool IsElementPresent(By by)
        {
            try
            {
                driver.FindElement(by);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        private bool IsAlertPresent()
        {
            try
            {
                driver.SwitchTo().Alert();
                return true;
            }
            catch (NoAlertPresentException)
            {
                return false;
            }
        }

        private string CloseAlertAndGetItsText()
        {
            try
            {
                IAlert alert = driver.SwitchTo().Alert();
                string alertText = alert.Text;
                if (acceptNextAlert)
                {
                    alert.Accept();
                }
                else
                {
                    alert.Dismiss();
                }
                return alertText;
            }
            finally
            {
                acceptNextAlert = true;
            }
        }
    }
}

