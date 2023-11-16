using MySql.Data.MySqlClient;
using WebAPI_db.Models;
using WebAPI_db.Utility;

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

        public Student? FindStudentById(int id)
        {
            Student student = null;
            MySqlDataReader reader = null;
            try
            {
                // query from db...
                string sqlCmd = string.Format("select * from student where id = {0} and deleted = 0", id);
                MySqlCommand cmd = DBAgent.Instance.BuildCmd(sqlCmd);
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    // should be only one
                    student = new Student();

                    student.Id = reader.GetInt32("id");
                    student.Name = reader.GetString("name");
                    student.PhoneNo = reader.GetString("phoneNo");
                    student.Note = reader.GetString("note");
                    student.Deleted = false;
                    break;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            if (reader != null) reader.Close();
            return student;
        }

        public List<Student> FindStudentByName(string name)
        {
            List<Student> list = new List<Student>();
            MySqlDataReader reader = null;
            try
            {
                // query from db...
                string sqlCmd = string.Format("select * from student where name = {0} and deleted = 0", name);
                MySqlCommand cmd = DBAgent.Instance.BuildCmd(sqlCmd);
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    // should be only one
                    Student student = new Student();
                    student.Id = reader.GetInt32("id");
                    student.Name = reader.GetString("name");
                    student.PhoneNo = reader.GetString("phoneNo");
                    student.Note = reader.GetString("note");
                    student.Deleted = false;
                    list.Add(student);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            if (reader != null) reader.Close();
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
