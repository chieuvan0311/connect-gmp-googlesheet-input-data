
using PAYPAL.Controller;
using PAYPAL.DataConnection;
using PAYPAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace PAYPAL.Wiews
{
    public partial class fAddNewAccounts : Form
    {
        DbContext db = null;
        public fAddNewAccounts()
        {
            InitializeComponent();
            this.CenterToScreen();
            db = new DbContext();
            grNewAccount_Adding.DataSource = new List<Add_New_Accounts_Model>();
            grNewAccount_Adding.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grNewAccount_Adding.Columns[0].Width = 50;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            grNewAccount_Adding.DataSource = new List<Add_New_Accounts_Model>();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            grNewAccount_Adding.DataSource = Add_New_Accounts_GoogleSheet();

        }

        public List<Add_New_Accounts_Model> Add_New_Accounts_GoogleSheet()
        {
            var google_sheet = new Google_Sheet_Controller().Get_New_Accounts_List();
            List<Add_New_Accounts_Model> list_table_acc = new List<Add_New_Accounts_Model>();
            if (google_sheet.Count > 0)
            {
                List<string> new_email_repeatation_list = new List<string>();
                List<Account> old_accounts_list = db.Accounts.ToList();
                int stt = 1;
                for (int i = 0; i < google_sheet.Count; i++)
                {
                    //check trùng với Acc cũ
                    Add_New_Accounts_Model table_acc = new Add_New_Accounts_Model();
                    Account add_database_acc = new Account();

                    string check_email = "";
                    try { check_email = google_sheet[i][0].ToString(); } catch { }

                    if (string.IsNullOrEmpty(check_email)) { continue; }

                    bool no_repeatation_01 = true;
                    if (old_accounts_list.Count > 0)
                    {
                        foreach (var old_account in old_accounts_list)
                        {
                            if (google_sheet[i][0].ToString() == old_account.Email)
                            {
                                no_repeatation_01 = false;
                                break;
                            }
                        }
                    }

                    if (no_repeatation_01 == false) //Trùng với Acc cũ
                    {
                        table_acc.STT = stt;
                        table_acc.Email = google_sheet[i][0].ToString();
                        try { table_acc.AccPassword = google_sheet[i][1].ToString(); } catch { }
                        try { table_acc.TwoFA = google_sheet[i][2].ToString(); } catch { }
                        try { table_acc.EmailPassword = google_sheet[i][3].ToString(); } catch { }
                        try { table_acc.Email_2FA = google_sheet[i][4].ToString(); } catch { }
                        try { table_acc.RecoveryEmail = google_sheet[i][5].ToString(); } catch { }
                        try { table_acc.ForwordToEmail = google_sheet[i][6].ToString(); } catch { }

                        table_acc.Status = "Trùng với Acc cũ!";
                        list_table_acc.Add(table_acc);

                        stt = stt + 1;
                    }
                    else //Không trùng acc cũ;
                    {
                        bool no_repeatation_02 = true;
                        for (int j = 0; j < google_sheet.Count; j++)  //check trùng acc mới
                        {
                            string google_email_02 = "";
                            try { google_email_02 = google_sheet[j][0].ToString(); } catch { }
                            if(string.IsNullOrEmpty(google_email_02)) { continue; }

                            if (j == i)
                            {
                                continue;
                            }
                            else
                            {
                                
                                if (check_email == google_email_02)
                                {
                                    if (new_email_repeatation_list.Count > 0)
                                    {
                                        bool add_email_list = true;
                                        foreach (string email in new_email_repeatation_list)
                                        {
                                            if (google_sheet[i][0].ToString() == email)
                                            {
                                                no_repeatation_02 = false; //Không phải là giá trị đầu tiên
                                                add_email_list = false;
                                                break;
                                            }
                                        }
                                        if (add_email_list == true)
                                        {
                                            new_email_repeatation_list.Add(google_sheet[j][0].ToString());
                                        }
                                    }
                                    else
                                    {
                                        new_email_repeatation_list.Add(google_sheet[j][0].ToString());
                                    }
                                    break;
                                }
                            }
                        }

                        if (no_repeatation_02 == false)
                        {
                            table_acc.STT = stt;
                            table_acc.Email = google_sheet[i][0].ToString();
                            try { table_acc.AccPassword = google_sheet[i][1].ToString(); } catch { }
                            try { table_acc.TwoFA = google_sheet[i][2].ToString(); } catch { }
                            try { table_acc.EmailPassword = google_sheet[i][3].ToString(); } catch { }
                            try { table_acc.Email_2FA = google_sheet[i][4].ToString(); } catch { }
                            try { table_acc.RecoveryEmail = google_sheet[i][5].ToString(); } catch { }
                            try { table_acc.ForwordToEmail = google_sheet[i][6].ToString(); } catch { }
                            try { table_acc.Proxy = google_sheet[i][7].ToString(); } catch { }
                            table_acc.Status = "Trùng với Acc mới!";
                            list_table_acc.Add(table_acc);
                            stt = stt + 1;
                        }
                        else //Trả kết quả về giao diện
                        {
                            table_acc.STT = stt;
                            table_acc.Email = google_sheet[i][0].ToString();
                            try { table_acc.AccPassword = google_sheet[i][1].ToString(); } catch { }
                            try { table_acc.TwoFA = google_sheet[i][2].ToString(); } catch { }
                            try { table_acc.EmailPassword = google_sheet[i][3].ToString(); } catch { }
                            try { table_acc.Email_2FA = google_sheet[i][4].ToString(); } catch { }
                            try { table_acc.RecoveryEmail = google_sheet[i][5].ToString(); } catch { }
                            try { table_acc.ForwordToEmail = google_sheet[i][6].ToString(); } catch { }

                            table_acc.Status = "Thêm Acc thành công!";
                            list_table_acc.Add(table_acc);
                            stt = stt + 1;

                            //Add vào database
                            add_database_acc.Email = google_sheet[i][0].ToString();
                            try { add_database_acc.AccPassword = google_sheet[i][1].ToString(); } catch { }
                            try { add_database_acc.TwoFA = google_sheet[i][2].ToString(); } catch { }
                            try { add_database_acc.EmailPassword = google_sheet[i][3].ToString(); } catch { }
                            try { add_database_acc.Email_2FA = google_sheet[i][4].ToString(); } catch { }
                            try { add_database_acc.RecoveryEmail = google_sheet[i][5].ToString(); } catch { }
                            try { add_database_acc.ForwordToEmail = google_sheet[i][6].ToString(); } catch { }
                            try { add_database_acc.Proxy = google_sheet[i][7].ToString(); } catch { }
                            add_database_acc.Acc_ON_OFF = true;
                            add_database_acc.AccType = "Chưa set";


                            string set_ChangedPassEmail = "";
                            string set_Add_RecoveryEmail = "";
                            string set_Deleted_FwEmail = "";
                            string set_Add_New_FwEmail = "";
                            try { set_ChangedPassEmail = google_sheet[i][9].ToString(); } catch { }
                            try { set_Add_RecoveryEmail = google_sheet[i][10].ToString(); } catch { }
                            try { set_Deleted_FwEmail = google_sheet[i][11].ToString(); } catch { }
                            try { set_Add_New_FwEmail = google_sheet[i][12].ToString(); } catch { }

                            if (set_ChangedPassEmail == "YES") { add_database_acc.Set_ChangedPassEmail = false; } else { add_database_acc.Set_ChangedPassEmail = true; }
                            if (set_Add_RecoveryEmail == "YES") { add_database_acc.Set_Add_RecoveryEmail = false; } else { add_database_acc.Set_Add_RecoveryEmail = true; }
                            if (set_Deleted_FwEmail == "YES") { add_database_acc.Set_Deleted_FwEmail = false; } else { add_database_acc.Set_Deleted_FwEmail = true; }
                            if (set_Add_New_FwEmail == "YES") { add_database_acc.Set_Add_New_FwEmail = false; } else { add_database_acc.Set_Add_New_FwEmail = true; }

                            var new_acc_email = google_sheet[i][0].ToString();
                            string email_type = null;
                            try { email_type = new_acc_email.Split('@')[1]; } catch { }
                            add_database_acc.EmailType = email_type;

                            db.Accounts.Add(add_database_acc);
                            db.SaveChanges();
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Goole sheet không có dữ liệu", "Thông báo");
            }

            return list_table_acc;
        }
    }
}
