using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace SchoolSiteTest
{
    public class Tests
    {
        private IWebDriver driver;

        private readonly By _signInButton = By.XPath("//span[text()='Войти']");
        private readonly By _loginInputButton = By.XPath("//input[@name='username']");
        private readonly By _passwordInputButton = By.XPath("//input[@name='password']");
        private readonly By _enterButton = By.XPath("//input[@class='button_gray']");
        private readonly By _errorLabel = By.XPath("//ul[@class='errorlist nonfield']");
        private readonly By _rightLabel = By.XPath("//span[text()='Дещеня Владислав']");

        private const string _errorLogin = "+375447335284";
        private const string _errorPassword = "12345678";
        private const string _expectedLabel1 = "Пожалуйста, введите правильные Логин и пароль. Оба поля могут быть чувствительны к регистру.";
        private const string _rightLogin = "+375293882443";
        private const string _rightPassword = "688влад2010";
        private const string _expectedLabel2 = "Дещеня Владислав";

        [SetUp]
        public void Setup()
        {
            driver = new ChromeDriver();
            driver.Navigate().GoToUrl("https://gymn2bobr.schools.by");
            driver.Manage().Window.Maximize();
        }

        [Test]
        public void IncorrectAuthorization()
        {
            var sygnIn = driver.FindElement(_signInButton);
            sygnIn.Click();

            var login = driver.FindElement(_loginInputButton);
            login.SendKeys(_errorLogin);

            var password = driver.FindElement(_passwordInputButton);
            password.SendKeys(_errorPassword);

            Thread.Sleep(1000);

            var enter = driver.FindElement(_enterButton);
            enter.Click();

            Thread.Sleep(1000);

            var actual = driver.FindElement(_errorLabel).Text;
            
            Assert.AreEqual(_expectedLabel1, actual, "u can authorizate:)");
        }

        [Test]
        public void CorrectAuthorization()
        {
            var sygnIn = driver.FindElement(_signInButton);
            sygnIn.Click();

            var login = driver.FindElement(_loginInputButton);
            login.SendKeys(_rightLogin);

            var password = driver.FindElement(_passwordInputButton);
            password.SendKeys(_rightPassword);

            Thread.Sleep(1000);

            var enter = driver.FindElement(_enterButton);
            enter.Click();

            Thread.Sleep(1000);

            var actual = driver.FindElement(_rightLabel).Text;

            Assert.AreEqual(_expectedLabel2, actual, "no");
        }

        [Test]
        public void BruteForce()
        {
            List<string> passwords = new List<string>();
            List<bool> expected = new List<bool>() { 
                false, false, false, false, false, false, false,
                false, false, false, false, false, false, false,
                false, false, false, false, false, false, false,
                false, false, false, false, false, false, false,
                false, false, false, false, false, false, true };
            List<bool> actual = new List<bool>();

            passwords = File.ReadAllLines("../../../../pass.txt").ToList();

            var sygnIn = driver.FindElement(_signInButton);
            sygnIn.Click();

            var login = driver.FindElement(_loginInputButton);
            login.SendKeys(passwords[0]);

            var password = driver.FindElement(_passwordInputButton);
            password.SendKeys(passwords[1]);

            Thread.Sleep(1000);

            var enter = driver.FindElement(_enterButton);
            enter.Click();

            Thread.Sleep(1000);

            try
            {
                driver.FindElement(_errorLabel);
                actual.Add(false);
            }
            catch
            {
                actual.Add(true);
            }

            for (int i = 2; i < passwords.Count; ++i)
            {
                password = driver.FindElement(By.XPath("//input[@id='id_password']"));
                password.SendKeys(passwords[i]);                

                enter = driver.FindElement(By.XPath("//input[@class='button_blue'][@value='Войти']"));
                enter.Click();

                Thread.Sleep(500);

                try
                {
                    driver.FindElement(_errorLabel);
                    actual.Add(false);
                }
                catch
                {
                    actual.Add(true);
                }
            }

            CollectionAssert.AreEqual(actual, actual);
        }

        [TearDown]
        public void TearDown()
        {
            driver.Quit();
        }
    }
}