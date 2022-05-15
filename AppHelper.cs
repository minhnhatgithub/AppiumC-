using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Appium
{

    public class DeviceEntity
    {
        public string DeviceId { get; set; }
        public int Port { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }

        public string User { get; set; }
        public string Pass { get; set; }

        public string FBUser { get; set; }

    }
    public class AppHelper
    {

        public static bool IsNumber(string s)
        {
            try
            {
                Convert.ToInt32(s);
                return true;
            }
            catch { return false; }
        }
        public static List<string> GetAllPortListeningOnComputer()
        {
            List<string> list = new List<string>();
            try
            {
                string cmd = string.Empty;
                string text = string.Empty;
                cmd = "netstat -an |find /i \"listening\"";
                text = RunCommand(cmd);
                string[] array = text.Split(new char[]
				{
					'\n'
				});
                foreach (string text2 in array)
                {
                    if (text2.Contains("127.0.0") || text2.Contains("0.0.0"))
                    {
                        MatchCollection matchCollection = Regex.Matches(text2, "(.*?)\\:(.*?) ", RegexOptions.Singleline);
                        if (matchCollection.Count > 0)
                        {
                            string text3 = matchCollection[0].Groups[2].Value.ToString().Trim();
                            if (IsNumber(text3))
                            {
                                list.Add(text3);
                            }
                        }
                    }
                }
            }
            catch (Exception exx)
            {

            }
            return list;
        }

        public static string RunCommand(string cmd)
        {
            string result = string.Empty;
            try
            {
                Process process = Process.Start(new ProcessStartInfo
                {
                    FileName = Environment.ExpandEnvironmentVariables("%SystemRoot%") + "\\System32\\cmd.exe",
                    WorkingDirectory = Application.StartupPath,
                    Arguments = "/c " + cmd,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    Verb = "runas",
                    RedirectStandardError = true
                });
                result = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                process.Close();
                process.Dispose();
            }
            catch (Exception)
            {

                result = "404";
            }
            return result;
        }

        public static RemoteWebDriver StartPhone(DeviceEntity device)
        {

            RemoteWebDriver driver = null;
            DesiredCapabilities dc = new DesiredCapabilities();
            dc.SetCapability("platformName", "Android");
            dc.SetCapability("platformVersion", device.Version.Split('.')[0]);
            dc.SetCapability("deviceName", device.DeviceId);
            dc.SetCapability("unicodeKeyboard", true);
            bool isInit = false;
            if (isInit)
            {
                dc.SetCapability("skipServerInstallation", true);
                dc.SetCapability("skipDeviceInitialization", true);
            }
            dc.SetCapability("systemPort", device.Port + 4000);
            dc.SetCapability("udid", device.DeviceId);
            dc.SetCapability("devicePort", device.Port);
            dc.SetCapability("noReset", true);
            dc.SetCapability("disableWindowAnimation", true);
            dc.SetCapability("gpsEnabled", true);
            dc.SetCapability("skipLogcatCapture", true);
            dc.SetCapability("dontStopAppOnReset", true);
            dc.SetCapability("ignoreHiddenApiPolicyError", true);
            dc.SetCapability("noSign", false);
            dc.SetCapability("newCommandTimeout", 36000);

            try
            {
                var lstPort = GetAllPortListeningOnComputer();
                int trycount = 0;
                while (trycount < 5 && !lstPort.Contains(device.Port.ToString()))
                {
                    trycount++;
                    if (!lstPort.Contains(device.Port.ToString()))
                    {
                        var appInfo = StartServer(device.DeviceId, device.Port);
                        if (appInfo.is_started)
                        {
                            lstPort = GetAllPortListeningOnComputer();

                            break;
                        }
                    }

                    Thread.Sleep(1000);
                }

                if (lstPort.Contains(device.Port.ToString()))
                {
                    try
                    {
                        driver = new RemoteWebDriver(new Uri("http://127.0.0.1:" + device.Port + "/wd/hub"), dc);
                        driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(5));
                    }
                    catch (Exception ex)
                    {
                        System.IO.File.AppendAllLines("appium.txt", new List<string>() { device.Name + " : " + ex.Message });
                    }
                    return driver;
                }
                return null;

            }
            catch (Exception ex)
            {
            }

            return driver;
        }

        public static AppiumInfo StartServer(string deviceId, int port)
        {


            AppiumInfo appiumInfo = new AppiumInfo();
            appiumInfo.pid = deviceId;
            appiumInfo.port = port;
            appiumInfo.is_started = false;
            appiumInfo.Id = 0;
            appiumInfo.SesionId = 0;
            try
            {
                string cmd = " appium --port " + port;
                int trycount = 0;
                new Task(() =>
                {
                     RunCommand(cmd);
                }).Start();
                Thread.Sleep(2000);

                while (trycount < 20)
                {
                    var lstPort = GetAllPortListeningOnComputer();
                    trycount++;
                    if (lstPort.Contains(port.ToString()))
                    {
                        appiumInfo.is_started = true;
                        appiumInfo.Message = "Success";
                        return appiumInfo;
                    }
                }

            }
            catch (Exception ex)
            {
                appiumInfo.Message = ex.Message;
            }
            return appiumInfo;
        }

        internal static void ExpandNotification()
        {
        }

        internal static void ExpandNotification(DeviceEntity d)
        {
            RunCommand(String.Format("adb -s {0} shell cmd statusbar expand-notifications", d.Name));
        }
    }
}
