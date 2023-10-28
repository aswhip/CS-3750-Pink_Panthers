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
    public class CreateDeleteClassTest
    {

        [TestMethod]
        public void CreateDeleteCourseTest()
        {
            IWebDriver driver = new ChromeDriver();

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

            IWebElement classLink = driver.FindElement(By.Id("ClassLink"));
            classLink.Click();

            System.Threading.Thread.Sleep(2000);

            IWebElement createClassLink = driver.FindElement(By.Id("CreateClass"));
            createClassLink.Click();

            System.Threading.Thread.Sleep(2000);

            IWebElement room = driver.FindElement(By.Id("Room"));
            room.SendKeys("NB 234");

            IWebElement department = driver.FindElement(By.Id("DepartmentCode"));
            department.SendKeys("CS");

            IWebElement courseNumber = driver.FindElement(By.Id("CourseNumber"));
            courseNumber.SendKeys("1400");

            IWebElement courseName = driver.FindElement(By.Id("CourseName"));
            courseName.SendKeys("Programming I");

            IWebElement Tuesday = driver.FindElement(By.Id("TuesdayCheckbox"));
            Tuesday.Click();

            IWebElement Thursday = driver.FindElement(By.Id("ThursdayCheckbox"));
            Thursday.Click();

            IWebElement Start = driver.FindElement(By.Id("StartTime"));
            Start.SendKeys("09301");

            IWebElement endTime = driver.FindElement(By.Id("EndTime"));
            endTime.SendKeys("11201");

            IWebElement hours = driver.FindElement(By.Id("hours"));
            hours.SendKeys("4");

            System.Threading.Thread.Sleep(4000);

            IWebElement submit = driver.FindElement(By.Id("submit"));
            submit.Click();

            System.Threading.Thread.Sleep(2000);

            classLink = driver.FindElement(By.Id("ClassLink"));
            classLink.Click();

            System.Threading.Thread.Sleep(1000);

            ReadOnlyCollection<IWebElement> deleteButtons = driver.FindElements(By.CssSelector("[id^='Delete_']"));

            int totalDeleteButtons = deleteButtons.Count;

            IWebElement lastDeleteButton = deleteButtons[totalDeleteButtons - 1];

            lastDeleteButton.Click();

            System.Threading.Thread.Sleep(4000); 


            driver.Quit();
        }
    }
}
