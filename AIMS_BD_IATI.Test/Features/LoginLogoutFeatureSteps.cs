using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using TechTalk.SpecFlow;

namespace AIMS_BD_IATI.Test.Features
{
    [Binding]
    public class LoginLogoutFeatureSteps : SeleniumBase
    {
        [Given(@"open a browser")]
        public void GivenOpenABrowser()
        {
            //driver = new ChromeDriver();
            ////d.Manage().Window.Maximize();
            //opt = driver.Manage();
            //timeouts = opt.Timeouts();
            //timeouts.ImplicitWait = TimeSpan.FromSeconds(10);

            //wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        [Given(@"goto http://localhost/IATIImportSite")]
        public void GivenGotoHttpLocalhostIATIImportSite()
        {
            driver.GoToUrl("");
        }

        [Given(@"input UserName = ""(.*)"", password = ""(.*)""")]
        public void GivenInputUserNamePassword(string userName, string password)
        {
            driver.FillTextById("username", userName);
            driver.FillTextById("password", password);
        }

        [Given(@"click login button")]
        public void GivenClickLoginButton()
        {
            driver.FindElementById("btnLogin").Click();
        }

        [Then(@"the DP selection page should appear")]
        public void ThenTheDPSelectionPageShouldAppear()
        {
            wait.Until(ExpectedConditions.UrlContains("#/0Begin"));

        }

        //logout
        [Given(@"loggen in browser window")]
        public void GivenLoggenInBrowserWindow()
        {
            GivenOpenABrowser();
            GivenGotoHttpLocalhostIATIImportSite();
            GivenInputUserNamePassword("admin", "123456");
            GivenClickLoginButton();
            ThenTheDPSelectionPageShouldAppear();
        }

        [When(@"click on user button")]
        public void WhenClickOnUserButton()
        {
            driver.FindElementById("li-user-dropdown-button").Click();
        }

        [When(@"click on logout button")]
        public void WhenClickOnLogoutButton()
        {
            driver.FindElementById("btn-logout").Click();
        }

        [Then(@"the login page should appear")]
        public void ThenTheLoginPageShouldAppear()
        {
            wait.Until(ExpectedConditions.UrlContains("#/login"));
        }


    }
}
