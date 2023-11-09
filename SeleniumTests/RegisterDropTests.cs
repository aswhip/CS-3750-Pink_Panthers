using OpenQA.Selenium.Edge;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.Chrome;

namespace SeleniumTests
{
    [TestClass]
    public class RegisterDropTests
    {
        [TestMethod]
        public void RegisterForClassTest()
        {
			//IWebDriver driver = new EdgeDriver();
			IWebDriver driver = new ChromeDriver();

			string url = "https://pinkpanthers3750.azurewebsites.net";
            driver.Manage().Window.Maximize();

            driver.Navigate().GoToUrl(url);

            IWebElement username = driver.FindElement(By.Id("usernameText"));
            username.SendKeys("teststudent@gmail.com");

            IWebElement password = driver.FindElement(By.Id("passwordText"));
            password.SendKeys("test1");

            IWebElement loginBtn = driver.FindElement(By.Id("loginBtn"));

            loginBtn.Click();

            System.Threading.Thread.Sleep(3000); //Sleep for 3 seconds to give the profile screen time to load

            IWebElement registerLink = driver.FindElement(By.Id("registerLink"));
            registerLink.Click();

            System.Threading.Thread.Sleep(2000); //Sleep for 2 seconds to give the profile screen time to load

            IWebElement unitTestCourse = driver.FindElement(By.Id("6registerBtn"));
            unitTestCourse.Click();

            System.Threading.Thread.Sleep(4000); //Sleep for 4 seconds so we can see that the unit test course has been registered

			registerLink = driver.FindElement(By.Id("registerLink"));
			registerLink.Click();

			System.Threading.Thread.Sleep(2000); //Sleep for 2 seconds to give the register screen time to load

			unitTestCourse = driver.FindElement(By.Id("6dropBtn"));
			unitTestCourse.Click();

			System.Threading.Thread.Sleep(4000); //Sleep for 4 seconds so we can see that the unit test course has been registered


			driver.Quit();

        }
    }
}
