
using MySql.Data.MySqlClient;

namespace WebAPI_db.Utility
{
    public class DBAgent
    {
        const string myConnectionString = "server=localhost;database=myjpa;uid=root;pwd=Girf@55825168;";

        static DBAgent mInstance = null;
        MySqlConnection mCnn;

        public static DBAgent Instance
        {
            get
            {
                if (mInstance == null)
                {
                    mInstance = new DBAgent();
                    mInstance.Connect();
                }
                return mInstance;
            }
        }

        protected DBAgent()
        {
            mCnn = new MySqlConnection(myConnectionString);
        }

        public string Connect()
        {
            string errMsg = null;
            try
            {
                if (mCnn == null) throw new Exception("null mCnn");
                mCnn.Open();
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }
            return errMsg;
        }

        public string Disconnect()
        {
            string errMsg = null;
            try
            {
                if (mCnn == null) throw new Exception("null mCnn");
                mCnn.Close();
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }
            return errMsg;
        }

    }
}
