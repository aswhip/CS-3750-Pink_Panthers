using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;

namespace SeleniumTests
{
	[TestClass]
	public class LoginSeleniumTest
	{
		[TestMethod]
		public void LoginTest()
		{
			IWebDriver driver = new EdgeDriver();
			string url = "https://localhost:7011/Accounts/Login";
			driver.Manage().Window.Maximize();

			driver.Navigate().GoToUrl(url);

			IWebElement username = driver.FindElement(By.Id("usernameText"));
			username.SendKeys("teststudent@gmail.com");

			IWebElement password = driver.FindElement(By.Id("passwordText"));
			password.SendKeys("test1");

			IWebElement loginBtn = driver.FindElement(By.Id("loginBtn"));

			loginBtn.Click();

            System.Threading.Thread.Sleep(4000); //Sleep for 4 seconds to see the profile screen


            driver.Quit();

		}
    }
}
