using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TieAppointment
{
    public class Program
    {
        const string TIEAppointment = "https://icp.administracionelectronica.gob.es/icpplustiem/index";
        static ChromeDriver driver;
        const string Email = "desaihiren2009@gmail.com",
            Passport = "V6697136",
            ApplicantName = "JEEGNASA HIRAN DESAI",
            PhoneNumber = "627914749",
            NIEOrDNI = "Z0419459J";

        static Program()
        {
            var path = Directory.GetParent(Environment.CurrentDirectory)?.Parent?.Parent?.FullName;
            driver = new ChromeDriver(path + @"\drivers\");
            driver.Manage().Window.Maximize();
        }

        public static void Main()
        {
            var continuePlaying = true;
            while (continuePlaying)
            {
                try
                {
                    driver.Manage().Cookies.DeleteAllCookies();
                    driver.Navigate().GoToUrl(TIEAppointment);

                    AcceptCookies();

                    if (!SelectCity() ||
                        !SelectOffice() ||
                        !GoNextAfterOffceSelection() ||
                        !EnterNIEAndName() ||
                        !CheckAppointment())
                    {
                        continue;
                    }
                    continuePlaying = CheckToContinue();
                }
                catch (Exception ex)
                {                    
                    Console.WriteLine(ex.Message);
                    Thread.Sleep(1 * 60 * 1000);
                }
            }

            PlaySound();

            var clickButton1 = driver.FindElement(By.Id("btnSiguiente"));
            ClickAndWaitForPageToLoad(clickButton1);

            FillApplicantDetails();

            clickButton1 = driver.FindElement(By.Id("btnSiguiente"));
            ClickAndWaitForPageToLoad(clickButton1);

            Console.WriteLine("Appointments are available");

            Console.ReadLine();

        }

        private static void AcceptCookies()
        {
            //Accept cookie
            var cookieSelector = driver.FindElement(By.Id("cookie_action_close_header"));
            if (cookieSelector != null && cookieSelector.Displayed)
            {
                cookieSelector.Click();
            }
        }

        private static bool SelectCity()
        {
            //Select Madrid
            var citySelector = driver.FindElement(By.Id("form"));
            var citySelectElement = new SelectElement(citySelector);
            citySelectElement.SelectByText("Madrid");
            var clickButton = driver.FindElement(By.Id("btnAceptar"));
            if (ClickAndWaitForPageToLoad(clickButton))
            {
                return false;
            }
            return true;
        }

        private static bool SelectOffice()
        {
            //Select office
            var appointmentTypeSelection = driver.FindElement(By.Id("tramiteGrupo[1]"));
            var appointmentTypeSelectElement = new SelectElement(appointmentTypeSelection);
            appointmentTypeSelectElement.SelectByText("POLICÍA-EXPEDICIÓN DE TARJETAS CUYA AUTORIZACIÓN RESUELVE LA DIRECCIÓN GENERAL DE MIGRACIONES");
            var clickButton = driver.FindElement(By.Id("btnAceptar"));
            if (ClickAndWaitForPageToLoad(clickButton))
            {
                return false;
            }
            return true;
        }

        private static bool GoNextAfterOffceSelection()
        {
            var clickButton = driver.FindElement(By.Id("btnEntrar"));
            if (ClickAndWaitForPageToLoad(clickButton))
            {
                return false;
            }
            return true;
        }

        private static bool EnterNIEAndName()
        {
            var nieInputElement = driver.FindElement(By.Id("txtIdCitado"));
            nieInputElement.Clear();
            nieInputElement.SendKeys(NIEOrDNI);

            var nameInputElement = driver.FindElement(By.Id("txtDesCitado"));
            nameInputElement.Clear();
            nameInputElement.SendKeys(ApplicantName);

            var clickButton = driver.FindElement(By.Id("btnEnviar"));
            if (ClickAndWaitForPageToLoad(clickButton))
            {
                return false;
            }
            return true;
        }

        private static bool CheckAppointment()
        {
            var clickButton = driver.FindElement(By.Id("btnEnviar"));
            if (ClickAndWaitForPageToLoad(clickButton))
            {
                return false;
            }
            return true;
        }

        private static bool CheckToContinue()
        {
            var messageElements = driver.FindElements(By.ClassName("mf-msg__info"));
            if (!messageElements.Any())
            {
                return false;
            }
            else
                foreach (var message in messageElements)
                {
                    if (message.Text.Contains("En este momento no hay citas disponibles"))
                    {
                        Thread.Sleep(45 * 1000);
                    }
                    else
                    {
                        return false;
                    }
                }
            return true;
        }

        private static void FillApplicantDetails()
        {
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
        }


        private static bool ClickAndWaitForPageToLoad(IWebElement element, int timeout = 10)
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
                Thread.Sleep(3000);
                return CheckSessionMessage();
            }
            catch (NoSuchElementException)
            {
                Console.WriteLine("Element with locator: '" + element.GetAttribute("id") + "' was not found in current context page.");
                throw;
            }
        }

        private static void ScrollToElement(IWebElement element)
        {
            Actions actions = new Actions(driver);
            actions.MoveToElement(element);
            actions.Perform();
        }

        private static void PlaySound()
        {
            var player = new SoundPlayer($"{Environment.CurrentDirectory}\\warning_alarm.wav");
            player.PlayLooping();
        }

        private static bool CheckSessionMessage()
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
