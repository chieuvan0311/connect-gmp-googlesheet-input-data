using Newtonsoft.Json.Linq;
using OpenQA.Selenium.Chrome;
using .ChromeDrivers;
using .DataConnection;
using .Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace .GPM_API
{
    public class Create_Profiles
    {
        DbContext db = null;
        public Create_Profiles()
        {
            db = new DbContext();
        }

        public Session_Result_Model Create(Account Acc)
        {
            Session_Result_Model result = new Session_Result_Model();
            var db_Account = db.Accounts.Where(x => x.ID == Acc.ID).FirstOrDefault();
            UndectedChromeDriver driver = null;
            result.Status = false;
            string profileId = "";
            bool create = false;
            GPMLoginAPI api = new GPMLoginAPI();
        
            try 
            {
                DateTime? dtime = null;
                if (Acc.Profile == true) 
                {
                    profileId = Acc.ProfileId;
                }
                else
                {
                    bool canvas = true;
                    if (Acc.Canvas_profile == false) { canvas = false; }
                    JObject createdResult = api.Create(Acc.Email, Acc.Proxy, canvas);
                    if (createdResult != null)
                    {
                        bool status = Convert.ToBoolean(createdResult["status"]);
                        if (status == true)
                        {
                            profileId = Convert.ToString(createdResult["profile_id"]);
                            create = true;
                            db_Account.Profile = true;
                            db_Account.ProfileId = profileId;
                            dtime = DateTime.Now;
                            db_Account.Profile_Created_Time = dtime;
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Kết nối GMP-Login lỗi!", "Thông báo", MessageBoxButtons.OK,
                                    MessageBoxIcon.None, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                    }
                }

                if (!string.IsNullOrEmpty(profileId)) 
                {
                    JObject startedResult = api.Start(profileId);
                    if (startedResult != null)
                    {
                        bool start_status = Convert.ToBoolean(startedResult["status"]);
                        if (start_status == true)
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
                            //options.AddArguments("--start-maximized");
                            driver = new UndectedChromeDriver(service, options);
                            if (create == true)
                            {
                                result.Value_01 = profileId;
                                result.Value_02 = dtime.ToString();
                            }
                            db_Account.ProxyStatus = "Good";
                            db.SaveChanges();
                            //return
                            result.Value_03 = "Good";
                            result.Status = true;
                        }
                        else
                        {
                            if (create == true) { api.Delete(profileId); }
                            db_Account.ProxyStatus = "Check";
                            db_Account.Profile = false;
                            db_Account.ProfileId = null;
                            db_Account.Profile_Created_Time = null;
                            db.SaveChanges();
                            result.Value_03 = "Check";
                            result.Value_01 = "Proxy/GMP-Login lỗi--";
                        }
                    }
                    else 
                    {
                        MessageBox.Show("Kết nối GMP-Login lỗi!", "Thông báo", MessageBoxButtons.OK,
                                    MessageBoxIcon.None, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                    }
                }
            }
            catch 
            {
                result.Value_01 = "Lỗi GMP-Login hoặc bị gián đoạn--";
            }
            result.Driver = driver;
            return result;
        }
    }
}
