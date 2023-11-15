using System.Collections.Generic;
using WebAPI_db.Models;

namespace WebAPI_db.Services
{
    public class StudentService
    {
        static StudentService? mInstance = null;

        public static StudentService Instance
        {
            get
            {
                if (mInstance == null) mInstance = new StudentService();
                return mInstance;
            }
        }

        public Student FindStudentByAccount(string account)
        {
            // query from db...

            return null;
        }

        public List<Student> FindStudentByName(string name)
        {
            List<Student> list = new List<Student>();
            // query from db...

            return list;
        }

        public string UpdateStudentInfo(int id, Student student)
        {
            string errMsg = null;
            // proceed...

            return errMsg;
        }


    }
}
