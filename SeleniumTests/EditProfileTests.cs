using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace SeleniumTests
{
    [TestClass]
    public class EditProfileTests
    {
        [TestMethod]
        public void TeacherCanEditProfile()
        {
            IWebDriver driver = new ChromeDriver();

            string url = "https://pinkpanthers3750.azurewebsites.net";
            driver.Manage().Window.Maximize();

            driver.Navigate().GoToUrl(url);

            IWebElement username = driver.FindElement(By.Id("usernameText"));
            username.SendKeys("teacher@weber.edu");

            IWebElement password = driver.FindElement(By.Id("passwordText"));
            password.SendKeys("password");

            IWebElement loginButton = driver.FindElement(By.Id("loginBtn"));
            loginButton.Click();

            System.Threading.Thread.Sleep(5000);

            IWebElement profileLink = driver.FindElement(By.Id("editProfileLink"));
            profileLink.Click();

            System.Threading.Thread.Sleep(2000);

            IWebElement editButton = driver.FindElement(By.Id("editButton"));
            editButton.Click();

            System.Threading.Thread.Sleep(1000);

            IWebElement city = driver.FindElement(By.Id("cityText"));
            city.SendKeys("Bornem");

            IWebElement saveButton = driver.FindElement(By.Id("saveButton"));
            saveButton.Click();

            System.Threading.Thread.Sleep(2000);

            
            city = driver.FindElement(By.Id("City"));
            Assert.AreEqual("Bornem", city.GetAttribute("value"));

            System.Threading.Thread.Sleep(2000);

            editButton = driver.FindElement(By.Id("editButton"));
            editButton.Click();

            System.Threading.Thread.Sleep(1000);

            city = driver.FindElement(By.Id("cityText"));
            city.Clear();

            saveButton = driver.FindElement(By.Id("saveButton"));
            saveButton.Click();

            System.Threading.Thread.Sleep(2000);

            driver.Quit();
        }
    }
}
