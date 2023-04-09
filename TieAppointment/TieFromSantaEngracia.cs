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
    public class TieAutomationForSantaEngracia : TieAutomationForDirectorGeneral
    {
        const string Nationality = "INDIA";
        public TieAutomationForSantaEngracia(ChromeDriver driver) 
            : base(driver)
        {

        }
        
        protected override bool SelectOffice()
        {
            //Select office
            var appointmentTypeSelection = driver.FindElement(By.Id("tramiteGrupo[1]"));
            var appointmentTypeSelectElement = new SelectElement(appointmentTypeSelection);
            appointmentTypeSelectElement.SelectByText("POLICIA-TOMA DE HUELLA (EXPEDICIÓN DE TARJETA), RENOVACIÓN DE TARJETA DE LARGA DURACIÓN Y DUPLICADO");
            var clickButton = driver.FindElement(By.Id("btnAceptar"));
            if (ClickAndWaitForPageToLoad(clickButton))
            {
                return false;
            }
            return true;
        }

        protected override bool EnterNIEAndName()
        {
            try
            {
                var nationality = driver.FindElement(By.Id("txtPaisNac"));
                if (nationality != null)
                {
                    var nationalitySelectElement = new SelectElement(nationality);
                    nationalitySelectElement.SelectByText(Nationality);
                }
            }
            finally { }
            return base.EnterNIEAndName();
        }
    }
}
