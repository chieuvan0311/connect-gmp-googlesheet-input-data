using Newtonsoft.Json.Linq;
using .DataConnection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace .GPM_API
{
    public class Check_API
    {
        DbContext db = null;
        public Check_API()
        {
            db = new DbContext();
        }
        
        public bool Start ()
        {
            bool result = false;
            string beeliant_profileID = db.Admins.Where(x => x.Name == "admin@beeliant.com").FirstOrDefault().Value;
            string api_url = db.Admins.Where(x => x.Name == "API_URL").FirstOrDefault().Value;
            GPMLoginAPI api = new GPMLoginAPI();

            JObject createdResult = api.Create("Check API");
            string createdProfileId = null;
            if (createdResult != null)
            {
                bool status = Convert.ToBoolean(createdResult["status"]);
                if (status == true)
                {
                    createdProfileId = Convert.ToString(createdResult["profile_id"]);
                    api.Delete(createdProfileId);
                    result = true;
                }
            }
            else
            {
                StringWriter strWriteLine_02 = new StringWriter();
                strWriteLine_02.WriteLine("- Chưa kết nối API với GMP-Login, chưa bật GMP-Login hoặc API URL không đúng");
                strWriteLine_02.WriteLine(" ");
                strWriteLine_02.WriteLine("- API URL: " + api_url);
                MessageBox.Show(strWriteLine_02.ToString(), "Check kết nối API");
            }
            return result;
        }

        public bool Check()
        {
            bool result = false;
            string beeliant_profileID = db.Admins.Where(x => x.Name == "admin@beeliant.com").FirstOrDefault().Value;
            string api_url = db.Admins.Where(x => x.Name == "API_URL").FirstOrDefault().Value;
            GPMLoginAPI api = new GPMLoginAPI();

            JObject createdResult = api.Create("Check API");
            string createdProfileId = null;
            if (createdResult != null)
            {
                bool status = Convert.ToBoolean(createdResult["status"]);
                if (status == true)
                {
                    createdProfileId = Convert.ToString(createdResult["profile_id"]);
                    api.Delete(createdProfileId);


                    StringWriter strWriteLine_01 = new StringWriter();
                    strWriteLine_01.WriteLine("- Đã kết nối API với GMP-Login!");
                    strWriteLine_01.WriteLine(" ");
                    strWriteLine_01.WriteLine("- API URL: " + api_url);
                    strWriteLine_01.WriteLine("- Beeliant ProfileId: ");
                    strWriteLine_01.WriteLine(beeliant_profileID);

                    MessageBox.Show(strWriteLine_01.ToString(), "Check kết nối API");
                    result = true;
                }
            }
            else
            {
                StringWriter strWriteLine_02 = new StringWriter();
                strWriteLine_02.WriteLine("- Chưa kết nối API với GMP-Login, chưa bật GMP-Login hoặc API URL không đúng");
                strWriteLine_02.WriteLine(" ");
                strWriteLine_02.WriteLine("- API URL: " + api_url);
                strWriteLine_02.WriteLine(beeliant_profileID);

                MessageBox.Show(strWriteLine_02.ToString(), "Check kết nối API");
            }
            return result;
        }
    }
}
