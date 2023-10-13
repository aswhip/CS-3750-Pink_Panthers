using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;

namespace Pink_Panthers_Test
{
	[TestClass]
	public class SeleniumTest
	{
		[TestMethod]
		public void TestingSelenium()
		{
			IWebDriver driver = new EdgeDriver();
			string url = "https://www.google.com";
			driver.Manage().Window.Maximize();

			driver.Navigate().GoToUrl(url);

			driver.Quit();

		}
	}
}
