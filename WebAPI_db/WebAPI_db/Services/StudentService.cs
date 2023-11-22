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

        public List<Student> GetAllStudent()
        {
            List<Student> list = new List<Student>();
            MySqlDataReader? reader = null;
            try
            {
                // query from db...
                string sqlCmd = "select * from student where deleted = 0";
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

        public Student? FindStudentById(int id)
        {
            Student? student = null;
            MySqlDataReader? reader = null;
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
            MySqlDataReader? reader = null;
            try
            {
                // query from db...
                string sqlCmd = string.Format("select * from student where name = '{0}' and deleted = 0", name);
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

        public string? Create(Student student)
        {
            string? errMsg = null;
            try
            {
                if (student.Name == null || student.Name.Length == 0) throw new Exception("invalid name");

                // query from db...
                string sqlCmd = string.Format("insert into `mydb`.`student` (`name`, `phoneNo`, `note`, `deleted`) values ('{0}', '{1}', '{2}', '0');", student.Name, student.PhoneNo, student.Note);
                MySqlCommand cmd = DBAgent.Instance.BuildCmd(sqlCmd);
                int iRow = cmd.ExecuteNonQuery();
            }
            catch(Exception ex)
            {
                errMsg = ex.Message;
            }
            return errMsg;
        }

        public string? Update(int id, Student student)
        {
            string? errMsg = null;
            try
            {
                Student? curStudent = this.FindStudentById(id);
                if (curStudent == null) throw new Exception(string.Format("no student for id:{0}", id));

                string strContent = "";
                if (curStudent.Name != student.Name)
                {
                    if (strContent.Length > 0) strContent += ", ";
                    strContent += string.Format("`name` = '{0}'", student.Name);
                }
                if (curStudent.PhoneNo != student.PhoneNo)
                {
                    if (strContent.Length > 0) strContent += ", ";
                    strContent += string.Format("`phoneNo` = '{0}'", student.PhoneNo);
                }
                if (curStudent.Note != student.Note)
                {
                    if (strContent.Length > 0) strContent += ", ";
                    strContent += string.Format("`note` = '{0}'", student.Note);
                }

                // query from db...
                string sqlCmd = string.Format("update `mydb`.`student` set {0} where (`id` = '{1}');", strContent, id);
                MySqlCommand cmd = DBAgent.Instance.BuildCmd(sqlCmd);
                int iRow = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }
            return errMsg;
        }

        public string? Delete(int id)
        {
            string? errMsg = null;
            try
            {
                Student? student = this.FindStudentById(id);
                if (student == null) throw new Exception(string.Format("no student for id:{0}", id));

                // query from db...
                string sqlCmd = string.Format("update `mydb`.`student` set `deleted` = '1' where (`id` = '{0}');", id);
                //string sqlCmd = string.Format("delete from `mydb`.`student` where (`id` = '{0}');", id);
                MySqlCommand cmd = DBAgent.Instance.BuildCmd(sqlCmd);
                int iRow = cmd.ExecuteNonQuery();
            }
            catch(Exception ex)
            {
                errMsg = ex.Message;
            }
            return errMsg;
        }
    }
}
