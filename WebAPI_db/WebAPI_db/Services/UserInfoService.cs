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

        public string? Create(string account,  string password, string name, int gender)
        {
            string? errMsg = null;
            try
            {
                if (account == null || account.Length == 0) throw new Exception("invalid account");
                if (password == null || password.Length == 0) throw new Exception("invalid password");
                if (name == null || name.Length == 0) name = account;
                gender = gender == 1 ? 1 : 0;

                // query from db...
                string sqlCmd = string.Format("insert into `mydb`.`user_info` (`account`, `password`, `name`, `gender`) values ('{0}', '{1}', '{2}', '{3}');", account, password, name, gender);//string.Format("insert into \"mydb\".\"user_info\" (\"account\", \"password\", \"name\", \"gender\") values (\"{0}\", \"{1}\", \"{2}\", \"{3}\");", account, password, name, gender);
                MySqlCommand cmd = DBAgent.Instance.BuildCmd(sqlCmd);
                int iRow = cmd.ExecuteNonQuery();
            }
            catch(Exception ex)
            {
                errMsg = ex.Message;
            }
            return errMsg;
        }

        public UserInfo? FindByAccount(string account)
        {
            UserInfo? userInfo = null;
            MySqlDataReader? reader = null;
            try
            {
                // query from db...
                string sqlCmd = string.Format("select * from user_info where account = '{0}'", account);
                MySqlCommand cmd = DBAgent.Instance.BuildCmd(sqlCmd);
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    // should be only one
                    userInfo = new UserInfo();

                    userInfo.Account = reader.GetString("account");
                    userInfo.Password = reader.GetString("password");
                    userInfo.Name = reader.GetString("name");
                    userInfo.Gender = reader.GetBoolean("gender") ? 1 : 0;
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

        public string? ChangePassword(string account, string newPassword)
        {
            string? errMsg = null;
            try
            {
                if (account == null || account.Length == 0) throw new Exception("invalid account");
                if (newPassword == null || newPassword.Length == 0) throw new Exception("invalid password");

                UserInfo? curUserInfo = this.FindByAccount(account);
                if (curUserInfo == null) throw new Exception(string.Format("no userInfo for {0}", account));

                string strContent = "";
                if (newPassword != curUserInfo.Password)
                {
                    if (strContent.Length > 0) strContent += ", ";
                    strContent += string.Format("`password` = '{0}'", newPassword);
                }
                //if (userInfo.Name != curUserInfo.Name)
                //{
                //    if (strContent.Length > 0) strContent += ", ";
                //    strContent += string.Format("`name` = '{0}'", userInfo.Name);
                //}
                //if (userInfo.Gender != curUserInfo.Gender)
                //{
                //    if (strContent.Length > 0) strContent += ", ";
                //    strContent += string.Format("`gender` = '{0}'", userInfo.Gender);
                //}

                // query from db...
                string sqlCmd = string.Format("update `mydb`.`user_info` set {0} where (`account` = '{1}');", strContent, account);
                MySqlCommand cmd = DBAgent.Instance.BuildCmd(sqlCmd);
                int iRow = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }
            return errMsg;
        }
    }
}
