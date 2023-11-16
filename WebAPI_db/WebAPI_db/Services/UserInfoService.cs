using MySql.Data.MySqlClient;
using WebAPI_db.Models;
using WebAPI_db.Utility;

namespace WebAPI_db.Services
{
    public class UserInfoService
    {
        static UserInfoService? mInstance = null;

        public static UserInfoService Instance
        {
            get
            {
                if (mInstance == null) mInstance = new UserInfoService();
                return mInstance;
            }
        }

        public UserInfo? FindByAccount(string account)
        {
            UserInfo? userInfo = null;
            MySqlDataReader reader = null;
            try
            {
                // query from db...
                string sqlCmd = string.Format("select * from user_info where account = \"{0}\"", account);
                MySqlCommand cmd = DBAgent.Instance.BuildCmd(sqlCmd);
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    // should be only one
                    userInfo = new UserInfo();

                    userInfo.Account = reader.GetString("account");
                    userInfo.Password = reader.GetString("password");
                    userInfo.Name = reader.GetString("name");
                    userInfo.Gender = reader.GetBoolean("gender") ? 0 : 1;
                    break;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            if (reader != null) reader.Close();
            return userInfo;
        }
    }
}
