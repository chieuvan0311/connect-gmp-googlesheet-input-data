using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;
using .DataConnection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using System.Threading;

namespace .Controller
{
    public class Google_Sheet_Controller
    {
        DbContext db = null;
        string driver_link_id = null;
        string data_sheet = null;
        string save_accounts_sheet = null;
        string inuput_data_sheet = null;
        string update_proxy_sheet = null;
        string update_accounts_info_sheet = null;
        string service_email = null;

        public Google_Sheet_Controller()
        {
            db = new DbContext();
            //get all name
        }

        public IList<IList<Object>> Get_New_Accounts_List()
        {
            SheetsService service = Authorize_GoogleApp_For_SheetsService();
            String range = inuput_data_sheet + "!A4:M";
            SpreadsheetsResource.ValuesResource.GetRequest request = service.Spreadsheets.Values.Get(driver_link_id, range);
            ValueRange response = request.Execute();
            IList<IList<Object>> getValues = response.Values;
            return getValues;
        }

        public IList<IList<Object>> Get_Update_Info_Accounts_List()
        {
            SheetsService service = Authorize_GoogleApp_For_SheetsService();
            String range = update_accounts_info_sheet + "!A4:N";
            SpreadsheetsResource.ValuesResource.GetRequest request = service.Spreadsheets.Values.Get(driver_link_id, range);
            ValueRange response = request.Execute();
            IList<IList<Object>> getValues = response.Values;
            return getValues;
        }

        public IList<IList<Object>> Get_Proxy_List()
        {
            SheetsService service = Authorize_GoogleApp_For_SheetsService();
            String range = update_proxy_sheet + "!A4:A";
            SpreadsheetsResource.ValuesResource.GetRequest request = service.Spreadsheets.Values.Get(driver_link_id, range);
            ValueRange response = request.Execute();
            IList<IList<Object>> getValues = response.Values;
            return getValues;
        }

        public void Update_Database ()
        {
            List<Account>  accounts = db.Accounts.ToList();

            var service = Authorize_GoogleApp_For_SheetsService();
            string google_sheet_range = data_sheet + "!A2:AH";
            IList<IList<Object>> objNeRecords = Database_Accounts_Transfer(accounts);

            int database_list_row_count = accounts.Count;

            int google_sheet_row_total = Get_Total_GoogleSheet_Row(driver_link_id, google_sheet_range);
            if (google_sheet_row_total > database_list_row_count) //Clear phần data dư thừa
            {
                string row_index = (database_list_row_count + 2).ToString();
                string clear_from_row = data_sheet + "!A" + row_index + ":AH";
                int total_clear_row = google_sheet_row_total - database_list_row_count;
                IList<IList<Object>> empty_model = Get_GoogleSheet_Empty_Model(total_clear_row);
                Clear_Sheet_Range_InBatch(empty_model, driver_link_id, clear_from_row, service);
            }
            Update_GoogleSheet_InBatch(objNeRecords, driver_link_id, google_sheet_range, service);
        }

        private SheetsService Authorize_GoogleApp_For_SheetsService()
        {
            ServiceAccountCredential credential;
            string[] Scopes = { SheetsService.Scope.Spreadsheets };
            string serviceAccountEmail = service_email;
            string jsonfile = "credentials.json";

            using (Stream stream = new FileStream(@jsonfile, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                credential = (ServiceAccountCredential)
                    GoogleCredential.FromStream(stream).UnderlyingCredential;

                var initializer = new ServiceAccountCredential.Initializer(credential.Id)
                {
                    User = serviceAccountEmail,
                    Key = credential.Key,
                    Scopes = Scopes
                };
                credential = new ServiceAccountCredential(initializer);
            }

            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Google Sheets API .NET Quickstart",
            });

            return service;
        }

        private int Get_Total_GoogleSheet_Row(string spreadsheetId, string range)
        {
            int totalRow = 0;
            SheetsService service = Authorize_GoogleApp_For_SheetsService();

            SpreadsheetsResource.ValuesResource.GetRequest request = service.Spreadsheets.Values.Get(spreadsheetId, range);
            ValueRange response = request.Execute();
            IList<IList<Object>> getValues = response.Values;
            if (getValues != null)
            {
                totalRow = getValues.Count;
            }
            return totalRow;
        }

        private IList<IList<Object>> Database_Accounts_Transfer (List<Account> accounts)
        {
            List<IList<Object>> row_list_data = new List<IList<Object>>();

            for (int i = 0; i < accounts.Count; i++)
            {
                IList<Object> row_data = new List<Object>();

                row_data.Add(i + 1); //Index 1

                if (!string.IsNullOrEmpty(accounts[i].Notification)) { row_data.Add(accounts[i].Notification); } else { row_data.Add(""); }
                if (!string.IsNullOrEmpty(accounts[i].UpdatedDateTime)) { row_data.Add(accounts[i].UpdatedDateTime); } else { row_data.Add(""); }
                if (!string.IsNullOrEmpty(accounts[i].AccPassword)) { row_data.Add(accounts[i].AccPassword); } else { row_data.Add(""); }
                if (!string.IsNullOrEmpty(accounts[i].TwoFA)) { row_data.Add(accounts[i].TwoFA); } else { row_data.Add(""); }
                if (!string.IsNullOrEmpty(accounts[i].EmailPassword)) { row_data.Add(accounts[i].EmailPassword); } else { row_data.Add(""); }

                if (!string.IsNullOrEmpty(accounts[i].Email_2FA)) { row_data.Add(accounts[i].Email_2FA); } else { row_data.Add(""); }
                if (!string.IsNullOrEmpty(accounts[i].Proxy)) { row_data.Add(accounts[i].Proxy); } else { row_data.Add(""); }
                if (!string.IsNullOrEmpty(accounts[i].AccName)) { row_data.Add(accounts[i].AccName); } else { row_data.Add(""); }
                if (!string.IsNullOrEmpty(accounts[i].AccBirthDay)) { row_data.Add(accounts[i].AccBirthDay); } else { row_data.Add(""); }
                if (!string.IsNullOrEmpty(accounts[i].Address)) { row_data.Add(accounts[i].Address); } else { row_data.Add(""); }

                row_list_data.Add(row_data);
            }
            return row_list_data;
        }

        private IList<IList<Object>> Get_GoogleSheet_Empty_Model(int row_total)
        {
            List<IList<Object>> empty_row_list = new List<IList<Object>>();
            string empty_cell = "";
            for (int i = 1; i <= row_total; i++)
            {
                IList<Object> empty_row = new List<Object>();
                for (int col = 1; col <= 34; col++)
                {
                    empty_row.Add(empty_cell);
                }
                empty_row_list.Add(empty_row);
            }
            return empty_row_list;
        }

        private void Update_GoogleSheet_InBatch(IList<IList<Object>> values, string spreadsheetId, string from_row, SheetsService service)
        {
            SpreadsheetsResource.ValuesResource.UpdateRequest request =
               service.Spreadsheets.Values.Update(new ValueRange() { Values = values }, spreadsheetId, from_row);

            //request.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOption.INSERTROWS;
            request.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;
            var response = request.Execute();
        }

        private void Clear_Sheet_Range_InBatch(IList<IList<Object>> values, string spreadsheetId, string clear_from_row, SheetsService service)
        {
            SpreadsheetsResource.ValuesResource.UpdateRequest request =
               service.Spreadsheets.Values.Update(new ValueRange() { Values = values }, spreadsheetId, clear_from_row);

            //request.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOption.INSERTROWS;
            request.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;
            var response = request.Execute();
        }
    }
}
