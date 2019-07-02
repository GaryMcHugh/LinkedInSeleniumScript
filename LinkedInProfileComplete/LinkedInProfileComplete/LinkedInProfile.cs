using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;

namespace LinkedInProfileComplete
{
    [TestClass]
    public class LinkedInProfile
    {
        [TestMethod]
        public void CompleteProfile()
        {
            var driver = new ChromeDriver();

            Register(driver);
            Thread.Sleep(2000);
            FillInProfileSnapshot(driver);
        }

        private static void Register(ChromeDriver driver)
        {
            //not perfect as LinkedIn use a recaptcha to prevent automated registration
            driver.Navigate().GoToUrl("https://www.linkedin.com/");
            driver.Manage().Window.Maximize();

            driver.FindElement(By.XPath("//*[@data-tracking-control-name='guest_homepage-basic_nav-header-join']")).Click();
            driver.FindElement(By.Id("first-name")).SendKeys("John");
            driver.FindElement(By.Id("last-name")).SendKeys("Doe");
            driver.FindElement(By.Id("join-email")).SendKeys("<email goes here>");
            driver.FindElement(By.Id("join-password")).SendKeys("<password goes here>");
            driver.FindElement(By.Id("submit-join-form-text")).Click();
        }
        private void FillInProfileSnapshot(ChromeDriver driver)
        {
            driver.FindElement(By.Id("location-postal")).SendKeys("F53PG19");
            driver.FindElement(By.XPath("//*[@data-control-name='continue']")).Click();

            Thread.Sleep(2000);

            driver.FindElement(By.Id("typeahead-input-for-title")).SendKeys("Software Developer");
            driver.FindElement(By.Id("typeahead-input-for-company")).SendKeys("Botsford and Sons");
            driver.FindElement(By.ClassName("onboarding-header__title")).Click();

            var industry = driver.FindElement(By.Id("work-industry"));
            var selectIndustry = new SelectElement(industry);
            selectIndustry.SelectByText("Information Technology & Services");
            driver.FindElement(By.XPath("//*[@data-control-name='continue']")).Click();
            Thread.Sleep(2000);

            var code = GetCodeFromEmail();

            driver.FindElement(By.Id("email-confirmation-input")).SendKeys(code);
            driver.FindElement(By.XPath("//*[@data-control-name='verify']")).Click();
            Thread.Sleep(2000);
            driver.FindElement(By.XPath("//*[@data-control-name='skip']")).Click();
            Thread.Sleep(2000);
            driver.FindElement(By.XPath("//*[@data-control-name='skip']")).Click();
        }

        private string GetCodeFromEmail()
        {
            var driverFireFox = new FirefoxDriver();
            driverFireFox.Navigate().GoToUrl("https://webmail.register365.com");
            Thread.Sleep(2000);

            driverFireFox.FindElement(By.Id("rcmloginuser")).SendKeys("<email login name goes here>");
            driverFireFox.FindElement(By.Id("rcmloginpwd")).SendKeys("<email login password goes here>");
            driverFireFox.FindElement(By.Id("rcmloginsubmit")).Click();

            Thread.Sleep(4000);

            var table = driverFireFox.FindElement(By.Id("messagelist"));
            var allElements = table.FindElements(By.ClassName("focused")).First();
            var subjectLine = allElements.Text;

            return Regex.Match(subjectLine, @"\d+").Value;
        }


        //not working in some instances.. going to use a firefox browser for the email code instead
        private static void OpenNewTab(ChromeDriver driver)
        {
            ((IJavaScriptExecutor)driver).ExecuteScript("window.open();");
            driver.SwitchTo().Window(driver.WindowHandles.Last());
        }

        private static void CloseTab(ChromeDriver driver)
        {
            ((IJavaScriptExecutor) driver).ExecuteScript("window.close();");
            driver.SwitchTo().Window(driver.WindowHandles.First());
        }
    }
}
