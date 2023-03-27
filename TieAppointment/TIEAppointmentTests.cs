using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.IO;
using System.Linq;
using System.Media;
using System.Threading;

namespace TieAppointment
{
    public class TIEAppointmentTests
    {
        const string TIEAppointment = "https://icp.administracionelectronica.gob.es/icpplustiem/index";
        IWebDriver driver;
        const string Email = "",
            Passport = "",
            ApplicantName = "",
            PhoneNumber = "",
            NIEOrDNI = "";

        [OneTimeSetUp]
        public void Setup()
        {
            var path = Directory.GetParent(Environment.CurrentDirectory)?.Parent?.Parent?.FullName;
            driver = new ChromeDriver(path + @"\drivers\");
            driver.Manage().Window.Maximize();
        }

        [Test]
        public void CheckIfAppointmentIsAvailable()
        {
            var continuePlaying = true;
            while (continuePlaying)
            {
                driver.Navigate().GoToUrl(TIEAppointment);

                //Accept cookie
                var cookieSelector = driver.FindElement(By.Id("cookie_action_close_header"));
                if (cookieSelector != null && cookieSelector.Displayed)
                {
                    cookieSelector.Click();
                }

                //Select Madrid
                var citySelector = driver.FindElement(By.Id("form"));
                var citySelectElement = new SelectElement(citySelector);
                citySelectElement.SelectByText("Madrid");
                var clickButton = driver.FindElement(By.Id("btnAceptar"));
                ClickAndWaitForPageToLoad(clickButton);

                //Select office
                var appointmentTypeSelection = driver.FindElement(By.Id("tramiteGrupo[1]"));
                var appointmentTypeSelectElement = new SelectElement(appointmentTypeSelection);
                appointmentTypeSelectElement.SelectByText("POLICÍA-EXPEDICIÓN DE TARJETAS CUYA AUTORIZACIÓN RESUELVE LA DIRECCIÓN GENERAL DE MIGRACIONES");
                clickButton = driver.FindElement(By.Id("btnAceptar"));
                ClickAndWaitForPageToLoad(clickButton);

                clickButton = driver.FindElement(By.Id("btnEntrar"));
                ClickAndWaitForPageToLoad(clickButton);

                var nieInputElement = driver.FindElement(By.Id("txtIdCitado"));
                nieInputElement.Clear();
                nieInputElement.SendKeys(NIEOrDNI);

                var nameInputElement = driver.FindElement(By.Id("txtDesCitado"));
                nameInputElement.Clear();
                nameInputElement.SendKeys(ApplicantName);

                clickButton = driver.FindElement(By.Id("btnEnviar"));
                ClickAndWaitForPageToLoad(clickButton);

                clickButton = driver.FindElement(By.Id("btnEnviar"));
                ClickAndWaitForPageToLoad(clickButton);

                var messageElements = driver.FindElements(By.ClassName("mf-msg__info"));
                if (!messageElements.Any())
                {
                    continuePlaying = false;
                }
                else
                    foreach (var message in messageElements)
                    {
                        if (message.Text.Contains("En este momento no hay citas disponibles"))
                        {
                            Thread.Sleep(60 * 1000);
                        }
                        else
                        {
                            continuePlaying = false;
                        }
                    }
            }
            PlaySound();

            var clickButton1 = driver.FindElement(By.Id("btnSiguiente"));
            ClickAndWaitForPageToLoad(clickButton1);

            var textInputElement = driver.FindElement(By.Id("txtTelefonoCitado"));
            textInputElement.Clear();
            textInputElement.SendKeys(PhoneNumber);

            textInputElement = driver.FindElement(By.Id("emailUNO"));
            textInputElement.Clear();
            textInputElement.SendKeys(Email);

            textInputElement = driver.FindElement(By.Id("emailDOS"));
            textInputElement.Clear();
            textInputElement.SendKeys(Email);

            textInputElement = driver.FindElement(By.Id("txtIdExtranjero"));
            textInputElement.Clear();
            textInputElement.SendKeys(Passport);

            textInputElement = driver.FindElement(By.Id("txtDesExtranjero"));
            textInputElement.Clear();
            textInputElement.SendKeys(ApplicantName);

            clickButton1 = driver.FindElement(By.Id("btnSiguiente"));
            ClickAndWaitForPageToLoad(clickButton1);

            Thread.Sleep(5 * 60 * 1000);
            Assert.IsTrue(true);
        }

        public bool ClickAndWaitForPageToLoad(IWebElement element, int timeout = 10)
        {
            try
            {
                if (element == null || !element.Displayed)
                {
                    throw new Exception("Cannot show element");
                }

                ScrollToElement(element);
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeout));
                element.Click();
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.StalenessOf(element));
                Thread.Sleep(5000);
                return CheckSessionMessage();
            }
            catch (NoSuchElementException)
            {
                Console.WriteLine("Element with locator: '" + element.GetAttribute("id") + "' was not found in current context page.");
                throw;
            }
        }
        private void ScrollToElement(IWebElement element)
        {
            Actions actions = new Actions(driver);
            actions.MoveToElement(element);
            actions.Perform();
        }

        private void PlaySound()
        {
            var player = new SoundPlayer($"{Environment.CurrentDirectory}\\warning_alarm.wav");
            player.PlayLooping();
        }

        private bool CheckSessionMessage()
        {

            var messageElements = driver.FindElements(By.ClassName("mf-msg__info"));
            if (!messageElements.Any())
            {
                return false;
            }
            foreach (var message in messageElements)
            {
                if (message.Text.Contains("Su sesión ha caducado por permanecer demasiado tiempo inactiva"))
                {
                    var clickButton = driver.FindElement(By.Id("btnSubmit"));
                    ClickAndWaitForPageToLoad(clickButton);
                    return true;
                }
            }
            return false;
        }
    }
}