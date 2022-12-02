using Newtonsoft.Json.Linq;
using .DataConnection;
using .Models;
using System;
using System.Linq;


namespace .GPM_API
{
    public class Delete_Profiles
    {
        DbContext db = null;
        public Delete_Profiles()
        {
            db = new DbContext();
        }

        public void Delete (int id, string profileId)
        {
            Session_Result_Model result = new Session_Result_Model();
            var db_Account = db.Accounts.Where(x => x.ID == id).FirstOrDefault();
            result.Status = false;
            GPMLoginAPI api = new GPMLoginAPI();
            api.Delete(profileId);
            db_Account.Profile_Created_Time = null;
            db_Account.Profile = false;
            db_Account.ProfileId = null;
            db_Account.Profile_Save = false;
            db.SaveChanges();
        }
    }
}
