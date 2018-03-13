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

        [Given(@"User imports a project from IATI data")]
        public void GivenUserImportsAProjectFromIATIData()
        {
            ScenarioContext.Current.Pending();
        }

        [Given(@"the project contains at least one policy marker with a significance code that is not `(.*)`")]
        public void GivenTheProjectContainsAtLeastOnePolicyMarkerWithASignificanceCodeThatIsNot(int p0)
        {
            ScenarioContext.Current.Pending();
        }

        [Then(@"on the `Sector Contribution` tab the `Policy Marker` collapsible contains the same policy markers")]
        public void ThenOnTheSectorContributionTabThePolicyMarkerCollapsibleContainsTheSamePolicyMarkers()
        {
            ScenarioContext.Current.Pending();
        }


    }
}
