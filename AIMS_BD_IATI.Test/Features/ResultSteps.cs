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
            ScenarioContext.Current.Pending();
        }

        [Then(@"on the `Results` tab the table contains at least one indicator\.")]
        public void ThenOnTheResultsTabTheTableContainsAtLeastOneIndicator_()
        {
            ScenarioContext.Current.Pending();
        }


    }
}
