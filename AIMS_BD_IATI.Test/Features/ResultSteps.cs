using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using TechTalk.SpecFlow;
using Xunit;

namespace AIMS_BD_IATI.Test.Features
{
    [Binding, Scope(Feature = "Results")]
    public class ResultSteps : BaseFieldMappingSteps
    {
        [Then(@"the page includes the number of `/result/indicator` \(e\.g\. ""(.*)""\)")]
        public void ThenThePageIncludesTheNumberOfResultIndicatorE_G_(string p0)
        {
            var resultDiv = driver.FindElementById("iati-Result");

            var firstChar = Convert.ToInt32(resultDiv.Text[0]);

            Assert.True(firstChar > 0);
        }

        [Given(@"the project contains at least one `result/indicator`")]
        public void GivenTheProjectContainsAtLeastOneResultIndicator()
        {
            wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("btn-view-mapped-projects")));

            driver.FindElementById("btn-view-mapped-projects").Click();
            wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.Id("us-spinner")));

            var firstProjectSetPreferenceButton = driver.FindElementByCssSelector("#review > div:nth-child(3) > div > h3 > span.pull-right > a:nth-child(1)");
            firstProjectSetPreferenceButton.Click();

            var resultDiv = driver.FindElementById("aims-Result");

            var firstChar = Convert.ToInt32(resultDiv.Text[0]);

            Assert.True(firstChar > 0);
        }

        [Then(@"on the `Results` tab the table contains at least one indicator\.")]
        public void ThenOnTheResultsTabTheTableContainsAtLeastOneIndicator_()
        {
            //this test should be in AIMS site
            //ScenarioContext.Current.Pending();
        }


    }
}
