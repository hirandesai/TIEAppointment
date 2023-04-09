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
        public static void Main()
        {
            var path = Directory.GetParent(Environment.CurrentDirectory)?.Parent?.Parent?.FullName;
            var driver = new ChromeDriver(path + @"\drivers\");
            driver.Manage().Window.Maximize();

            var continuePlaying = true;
            var tieAutomation = new TieAutomationForDirectorGeneral(driver);
            var tieAutomationSantaEn = new TieAutomationForSantaEngracia(driver);
            while (continuePlaying)
            {
                continuePlaying = tieAutomation.Start() && 
                                    tieAutomationSantaEn.Start();
            }
            tieAutomation.AfterExecutionComplete();
            Console.WriteLine("Appointments are available");
            Console.ReadLine();
        }

        
    }
}
