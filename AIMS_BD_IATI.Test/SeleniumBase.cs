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
    public abstract class SeleniumBase
    {
        public static string baseURL = "http://localhost/IATIImportSite/";

        public RemoteWebDriver driver;

        public bool acceptNextAlert = true;

        public IOptions opt;
        public ITimeouts timeouts;
        public WebDriverWait wait;

        [BeforeScenario]
        public void BeforeScenario()
        {
            //if (driver == null)
            //{
            //    StartDriver();
            //}
        }

        private void StartDriver()
        {
            driver = new ChromeDriver();
            opt = driver.Manage();
            timeouts = opt.Timeouts();
            timeouts.ImplicitWait = TimeSpan.FromSeconds(10);

            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
        }

        [AfterScenario]
        public void AfterScenario()
        {
            try
            {
                if (driver != null)
                {
                    driver.Quit();
                }
            }
            catch (Exception)
            {
                // Ignore errors if unable to close the browser
            }
        }

        #region Common Features
        //public void LoginAsPublic()
        //{
        //    GoToUrl("");

        //    wait.Until(ExpectedConditions.ElementIsVisible(By.Id("btn-logon-public")))
        //    .Click();

        //    wait.Until(ExpectedConditions.UrlToBe(baseURL + "AIMS/Home"));
        //}

        public void LoginAsAdmin()
        {
            GoToUrl("");

            driver.FillTextById("username", "admin");
            driver.FillTextById("password", "123456");

            driver.FindElementById("btnLogin").Click();

            wait.Until(ExpectedConditions.UrlContains("#/0Begin"));
        }

        //[Given(@"User edits a project")]
        //public void UserEditsFirstProject()
        //{
        //    LoginAsAdmin();

        //    GoToUrl("/AIMS/ProjectInfo/Index");

        //    var firstProjectEditButton = driver.FindElementByCssSelector("#projectInfoKendoGrid > div.k-grid-content > table > tbody > tr:nth-child(1) > td:nth-child(19) > a.lnkEditProjectInfo");

        //    firstProjectEditButton.Click();


        //}
        #endregion

        #region Helpers
        public void GoToUrl(string urlWithoutBase)
        {
            if (driver == null)
            {
                StartDriver();
            }

            driver.Navigate().GoToUrl(baseURL + urlWithoutBase);
        }

        public bool IsElementPresent(By by)
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

        public bool IsElementVisible(By by)
        {
            try
            {
                return driver.FindElement(by).Displayed;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool IsAlertPresent()
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

        public IWebElement J(string cssSelector)
        {
            return wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(cssSelector)));
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
        #endregion
    }
}

