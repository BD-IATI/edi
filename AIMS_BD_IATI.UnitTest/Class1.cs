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

namespace AIMS_BD_IATI.Test
{
    public class SeleniumBase
    {
        public RemoteWebDriver driver;
        private StringBuilder verificationErrors;
        //private string baseURL;
        private bool acceptNextAlert = true;

        IOptions opt;
        ITimeouts timeouts;
        public WebDriverWait wait;

        //public TestContext TestContext { get; set; }

        //[SetUp]
        public void SetupTest()
        {
            //baseURL = "http://localhost:53013/";
            verificationErrors = new StringBuilder();

            driver = new ChromeDriver();
            //d.Manage().Window.Maximize();
            opt = driver.Manage();
            timeouts = opt.Timeouts();
            timeouts.ImplicitWait = TimeSpan.FromSeconds(10);

            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            Login();

        }

        //[TearDown]
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
            Assert.Equal("", verificationErrors.ToString());
        }

        public void Login()
        {

            driver.GoToUrl("Account/Login");

            //driver.FillText(nameof(LoginRequest.Username), "admin");
            //driver.FillText(nameof(LoginRequest.Password), "12345678");

            driver.Keyboard.PressKey(Keys.Enter);

            wait.Until(ExpectedConditions.TitleContains("Dashboard"));


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

