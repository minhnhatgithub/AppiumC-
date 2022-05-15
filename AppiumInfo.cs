using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appium
{
    public class AppiumInfo
    {
        public bool is_started { get; set; }
        public string pid { get; set; }
        public int port { get; set; }
        public int Id { get; set; }
        public int SesionId { get; set; }
        public string Message { get; set; }
    }

    public class PhoneInfo
    {

        public RemoteWebDriver driver { get; set; }


        public string platformName { get; set; }


        public string deviceName { get; set; }


        public string udid { get; set; }

        public bool is_running { get; set; }


        public string message { get; set; }


        public string port { get; set; }





        private string paramsValue;


        private string orderValue;


        private string composerValue;


        private bool expressionValue;


        private string _ManagerValue;


        private string _ResolverValue;


        public int width = 0;


        public int heigth = 0;
    }
}
