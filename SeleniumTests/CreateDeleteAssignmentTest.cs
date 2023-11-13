using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace SeleniumTests
{
	[TestClass]
	public class CreateDeleteAssignmentTest
	{

		[TestMethod]
		public void CreateDeleteAssignment()
		{
			IWebDriver driver = new ChromeDriver();

			//string url = "https://localhost:7011/Accounts/Login";
			string url = "https://pinkpanthers3750.azurewebsites.net";

            driver.Manage().Window.Maximize();

			driver.Navigate().GoToUrl(url);

			IWebElement username = driver.FindElement(By.Id("usernameText"));
			username.SendKeys("testteacher@gmail.com");

			IWebElement password = driver.FindElement(By.Id("passwordText"));
			password.SendKeys("test1");

			IWebElement loginBtn = driver.FindElement(By.Id("loginBtn"));

			loginBtn.Click();

			System.Threading.Thread.Sleep(3000);

			IWebElement course = driver.FindElement(By.Id("REC 9999"));
			course.Click();

			System.Threading.Thread.Sleep(2000);

			IWebElement createAssignment = driver.FindElement(By.Id("CreateAssignment"));
			createAssignment.Click();

			System.Threading.Thread.Sleep(2000);

			IWebElement name = driver.FindElement(By.Id("AssignmentName"));
			name.SendKeys("Test Assignment");

			IWebElement description = driver.FindElement(By.Id("Description"));
			description.SendKeys("Selenium test assignment");

			IWebElement points = driver.FindElement(By.Id("PossiblePoints"));
			points.SendKeys("100");

			IWebElement due = driver.FindElement(By.Id("DueDate"));
			due.SendKeys("12/10/2023");

			IWebElement submission = driver.FindElement(By.Id("SubmissionType"));
			submission.Click();

			IWebElement create = driver.FindElement(By.Id("CreateAssignment"));
			create.Click();

            System.Threading.Thread.Sleep(2000);

            IWebElement view = driver.FindElement(By.Id("6assignmentBtn"));
            view.Click();

            System.Threading.Thread.Sleep(2000);

            IWebElement delete = driver.FindElement(By.Id("Test AssignmentdeleteBtn"));
            delete.Click();

            System.Threading.Thread.Sleep(4000);

            driver.Quit();
		}
	}
}