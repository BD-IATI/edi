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

        public static void FillText(this RemoteWebDriver d, string inputCssSelector, string value)
        {
            var element = d.FindElementByCssSelector(inputCssSelector);
            element.Clear();
            element.SendKeys(value);
        }
        public static void FillKendoNumeric(this RemoteWebDriver d, string inputId, string value)
        {
            var input1 = d.FindElementByXPath($"//input[@id='{inputId}']/preceding-sibling::input[1]");

            input1.Click();

            var element = d.FindElementById(inputId);
            element.Clear();
            element.SendKeys(value);
        }
        public static void FillDate(this RemoteWebDriver d, string inputCssSelector, string value)
        {
            var element = d.FindElementByCssSelector(inputCssSelector);
            element.Click();
            element.SendKeys(Keys.ArrowLeft);
            element.SendKeys(Keys.ArrowLeft);
            element.SendKeys(value);
        }
        public static void FillTextByName(this RemoteWebDriver d, string inputName, string value)
        {
            var element = d.FindElementByName(inputName);
            element.Clear();
            element.SendKeys(value);
        }

        public static void FillTextById(this RemoteWebDriver d, string inputId, string value)
        {
            var element = d.FindElementById(inputId);
            element.Clear();
            element.SendKeys(value);
        }

        public static void Select2LookupItem(this RemoteWebDriver d, string fieldName, string text)
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

        public static void SelectLookupItemByText(this RemoteWebDriver d, string selectId, string text)
        {
            SelectElement select = new SelectElement(d.FindElementById(selectId));
            select.SelectByText(text);
        }

        public static void SelectFirstLookupItem(this RemoteWebDriver d, string selectId)
        {
            var sss = d.FindElementById(selectId);

            SelectElement select = new SelectElement(sss);

            select.SelectByIndex(1);

        }

        public static void SelectFirstLookupItem(this IWebElement d, string selectId)
        {
            var sss = d.FindElement(By.Id(selectId));

            SelectElement select = new SelectElement(sss);

            select.SelectByIndex(1);


        }




        


    }
}

