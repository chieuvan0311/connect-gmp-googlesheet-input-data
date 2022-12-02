using Newtonsoft.Json.Linq;
using OpenQA.Selenium.Chrome;
using .ChromeDrivers;
using .DataConnection;
using .Models;
using .RandomData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace .GPM_API
{
    public class Open_Profiles
    {
        public Session_Result_Model Open (string profileId)
        {
            Session_Result_Model result = new Session_Result_Model();
            UndectedChromeDriver driver = null;
            result.Status = false;
            try
            {
                GPMLoginAPI api = new GPMLoginAPI();
                JObject startedResult = api.Start(profileId);
                if (startedResult != null)
                {
                    bool status = Convert.ToBoolean(startedResult["status"]);
                    if (status == true)
                    {
                        string browserLocation = Convert.ToString(startedResult["browser_location"]);
                        string seleniumRemoteDebugAddress = Convert.ToString(startedResult["selenium_remote_debug_address"]);
                        string gpmDriverPath = Convert.ToString(startedResult["selenium_driver_location"]);
                        // Init selenium
                        FileInfo gpmDriverFileInfo = new FileInfo(gpmDriverPath);
                        ChromeDriverService service = ChromeDriverService.CreateDefaultService(gpmDriverFileInfo.DirectoryName, gpmDriverFileInfo.Name);
                        service.HideCommandPromptWindow = true;
                        ChromeOptions options = new ChromeOptions();
                        options.BinaryLocation = browserLocation;
                        options.DebuggerAddress = seleniumRemoteDebugAddress;
                        driver = new UndectedChromeDriver(service, options);
                        result.Status = true;
                    }
                }
                else
                {
                    MessageBox.Show("Kết nối GMP-Login lỗi!", "Thông báo", MessageBoxButtons.OK,
                                        MessageBoxIcon.None, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                }
            }
            catch { }
            Thread.Sleep(RdTimes.T_600());
            result.Driver = driver;
            return result;
        }
    }
}
