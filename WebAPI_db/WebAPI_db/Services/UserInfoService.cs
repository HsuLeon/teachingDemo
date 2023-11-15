using WebAPI_db.Utility;
using WebAPI_put.Models;

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

        public UserInfo FindByAccount(string account)
        {
            UserInfo userInfo = null;
            // query from db...

            DBAgent dbAgent = DBAgent.Instance;

            return userInfo;
        }
    }
}
