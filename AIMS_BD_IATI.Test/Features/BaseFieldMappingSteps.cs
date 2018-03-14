using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using TechTalk.SpecFlow;
using Xunit;

namespace AIMS_BD_IATI.Test.Features
{

    public abstract class BaseFieldMappingSteps : SeleniumBase
    {
        [Given(@"User uses the IATI import module to import a project for DP=`(.*)`")]
        public void GivenUserUsesTheIATIImportModuleToImportAProject(string dp)
        {
            LoginAsAdmin();
            driver.SelectLookupItemByText("beginDPselect", dp);
            driver.FindElementById("btnDashboard").Click();
            wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.Id("us-spinner")));
        }

        [Given(@"User proceeds to the `(.*)\. Set import preferences` step")]
        public void GivenUserProceedsToThe_SetImportPreferencesStep(int p0)
        {

            GoToUrl("#/1Hierarchy");
            wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.Id("us-spinner")));
            GoToUrl("#/3FilterDP");
            wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.Id("us-spinner")));
            GoToUrl("#/4Projects");
            wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.Id("us-spinner")));
            GoToUrl("#/6GeneralPreferences");
            wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.Id("us-spinner")));



        }

        [Given(@"User imports a project from IATI data")]
        public void GivenUserImportsAProjectFromIATIData()
        {
            driver.FindElementById("btn-view-mapped-projects").Click();
            wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.Id("us-spinner")));
            driver.FindElementById("btn-import-projects").Click();
            wait.Until(ExpectedConditions.AlertIsPresent());

            driver.SwitchTo().Alert().Dismiss();

        }

    }
}
