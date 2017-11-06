using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;

namespace AIMS_BD_IATI.Test
{
    public static class SeleniumExtentions
    {
        static string baseURL = "http://localhost/IATIImportSite/";

        public static void FillText(this RemoteWebDriver d, string fieldName, string value)
        {
            var element = d.FindElementByName(fieldName);
            element.Clear();
            element.SendKeys(value);
        }

        public static void SelectLookupItem(this RemoteWebDriver d, string fieldName, string text)
        {
            var divField = d.FindElementByCssSelector(".field." + fieldName);

            var select2Container = divField.FindElement(By.ClassName("select2-container"));
            select2Container.Click();

            string subContainerClass = "#select2-drop:not([style*='display: none'])";
            var searchBox = d.FindElement(By.CssSelector(subContainerClass + " .select2-input"));
            searchBox.SendKeys(text);

            var selectedItem = d.FindElements(By.CssSelector(subContainerClass + " .select2-results li.select2-result-selectable")).First();
            selectedItem.Click();
        }

        public static void GoToUrl(this RemoteWebDriver d, string urlWithoutBase)
        {
            d.Navigate().GoToUrl(baseURL + urlWithoutBase);
        }

        public static void ClickMainGridAddButton(this RemoteWebDriver d)
        {
            d.FindElementByClassName("add-button").Click();
        }
        public static void ClickButton(this RemoteWebDriver d, string caption)
        {
            d.FindElementByClassName(caption).Click();
        }
    }
}

