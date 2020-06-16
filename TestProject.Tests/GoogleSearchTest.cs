using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace TestProject.Tests.Tests
{
    public class GoogleSearchTest
    {
        private ChromeDriver driver;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            driver = new ChromeDriver();
        }
        
        [Test]
        public void When_searching_google_should_find_something()
        {
            OpenGooglePage();
            EnterTextToInput();
            ClickOk();
            var searchResults = GetSearchResults();
            
            Assert.That(searchResults, Is.Not.Empty);
        }

        private List<string> GetSearchResults()
        {
            return driver.FindElements(By.XPath("//h3")).Select(x => x.Text).ToList();
         
        }

        private void ClickOk()
        {
            var okBtn = driver.FindElement(By.Name("btnK"));
            okBtn.Click();
        }

        private void EnterTextToInput()
        {
            var findElement = driver.FindElement(By.XPath("//input[@type='text' and @title='Search']"));
            findElement.SendKeys("test");
            Thread.Sleep(1000);
        }

        private void OpenGooglePage()
        {
            driver.Navigate().GoToUrl("https://google.com");
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            driver?.Dispose();
        }
    }
}