using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using TechTalk.SpecFlow;

namespace AIMS_BD_IATI.Test.Features
{
    [Binding]
    public class ResultSteps : SeleniumBase
    {
        [Then(@"the page includes the number of `/result/indicator` \(e\.g\. ""(.*)""\)")]
        public void ThenThePageIncludesTheNumberOfResultIndicatorE_G_(string p0)
        {
            ScenarioContext.Current.Pending();
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
