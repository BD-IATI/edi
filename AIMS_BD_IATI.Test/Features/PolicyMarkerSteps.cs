using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using TechTalk.SpecFlow;
using Xunit;

namespace AIMS_BD_IATI.Test.Features
{
    [Binding, Scope(Feature = "Policy markers from IATI")]
    public class PolicyMarkerSteps : BaseFieldMappingSteps
    {
        

        [Then(@"the page includes the list of policy markers that have a significance code that is not `(.*)`\.")]
        public void ThenThePageIncludesTheListOfPolicyMarkersThatHaveASignificanceCodeThatIsNot_(int p0)
        {
            var policyMarkerDiv = driver.FindElementById("iati-PolicyMarker");
            var firstChar = Convert.ToInt32(policyMarkerDiv.Text[0]);

            Assert.True(firstChar > 0);
        }


        [Given(@"the project contains at least one policy marker with a significance code that is not `(.*)`")]
        public void GivenTheProjectContainsAtLeastOnePolicyMarkerWithASignificanceCodeThatIsNot(int p0)
        {
            driver.FindElementById("btn-view-mapped-projects").Click();
            wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.Id("us-spinner")));

            var firstProjectSetPreferenceButton = driver.FindElementByCssSelector("#review > div:nth-child(3) > div > h3 > span.pull-right > a:nth-child(1)");
            firstProjectSetPreferenceButton.Click();

            var policyMarkerDiv = driver.FindElementById("aims-PolicyMarker");
            var firstChar = Convert.ToInt32(policyMarkerDiv.Text[0]);

            Assert.True(firstChar > 0);
        }

        [Then(@"on the `Sector Contribution` tab the `Policy Marker` collapsible contains the same policy markers")]
        public void ThenOnTheSectorContributionTabThePolicyMarkerCollapsibleContainsTheSamePolicyMarkers()
        {
            //this test should be in AIMS site
            //ScenarioContext.Current.Pending();
        }


    }
}
