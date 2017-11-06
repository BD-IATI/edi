using System;
using TechTalk.SpecFlow;

namespace AIMS_BD_IATI.Test.Features
{
    [Binding]
    public class SpecFlowFeature1Steps
    {
        [Given(@"open a browser")]
        public void GivenOpenABrowser()
        {
            ScenarioContext.Current.Pending();
        }
        
        [Given(@"goto http://localhost/IATIImportSite")]
        public void GivenGotoHttpLocalhostIATIImportSite()
        {
            ScenarioContext.Current.Pending();
        }
        
        [Given(@"input UserName = ""(.*)"", password = ""(.*)""")]
        public void GivenInputUserNamePassword(string userName, string password)
        {
            ScenarioContext.Current.Pending();
        }
        
        [Given(@"click login button")]
        public void GivenClickLoginButton()
        {
            ScenarioContext.Current.Pending();
        }
        
        [Then(@"the DP selection page should appear")]
        public void ThenTheDPSelectionPageShouldAppear()
        {
            ScenarioContext.Current.Pending();
        }
    }
}
