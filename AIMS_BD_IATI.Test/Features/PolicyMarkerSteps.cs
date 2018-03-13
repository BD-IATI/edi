using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using TechTalk.SpecFlow;

namespace AIMS_BD_IATI.Test.Features
{
    [Binding]
    public class PolicyMarkerSteps : SeleniumBase
    {
        [Given(@"User uses the IATI import module to import a project")]
        public void GivenUserUsesTheIATIImportModuleToImportAProject()
        {
            
        }

        [Given(@"User proceeds to the `(.*)\. Set import preferences` step")]
        public void GivenUserProceedsToThe_SetImportPreferencesStep(int p0)
        {
            ScenarioContext.Current.Pending();
        }

        [Then(@"the page includes the list of policy markers that have a significance code that is not `(.*)`\.")]
        public void ThenThePageIncludesTheListOfPolicyMarkersThatHaveASignificanceCodeThatIsNot_(int p0)
        {
            ScenarioContext.Current.Pending();
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
