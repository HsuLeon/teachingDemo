using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebAPI_post.Models;

namespace WebAPI_post.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {

        [HttpPost("querys")]
        public string Post1([FromQuery] string name, [FromQuery] int age)
        {
            return string.Format("the name is {0}, age is {1}", name, age);
        }

        [HttpPost("route/name/{name}/age/{age}")]
        public string Post2([FromRoute] string name, [FromRoute] int age)
        {
            return string.Format("the name is {0}, age is {1}", name, age);
        }

        [HttpPost("header")]
        public string Post3([FromHeader] string name, [FromHeader] int age)
        {
            return string.Format("the name is {0}, age is {1}", name, age);
        }

        [HttpPost("body/form")]
        public Student Post4([FromForm] Student student)
        {
            string name = student.name;
            int age = student.age;

            Student newStudent = new Student();
            newStudent.name = name + "_server";
            newStudent.age = age + 10;
            return newStudent;
        }

        [HttpPost("body/class")]
        public IActionResult Post5([FromBody] Student student)
        {
            string name = student.name;
            int age = student.age;

            Student newStudent = new Student();
            newStudent.name = name + "_server";
            newStudent.age = age + 10;
            return Ok(newStudent);
        }

        [HttpPost("body/class2")]
        public IActionResult Post6([FromForm] Student student)
        {
            string name = student.name;
            int age = student.age;

            if (age >= 18)
            {
                Student newStudent = new Student();
                newStudent.name = name + "_server";
                newStudent.age = age + 10;
                return Ok(newStudent);
            }
            else
            {
                return BadRequest("not adult yet");
            }
        }

        [HttpPost("body/json")]
        public IActionResult Post7([FromHeader] string name, [FromHeader] int age, [FromBody] JObject obj)
        {
            try
            {
                string nameInObj = obj.ContainsKey("name") ? obj["name"].Value<string>() : null;
                string ageInObj = obj.ContainsKey("age") ? obj["age"].Value<string>() : null;
                if (nameInObj == null) throw new Exception("null name");
                if (ageInObj == null) throw new Exception("null age");

                int iAge = int.Parse(ageInObj);

                Student newStudent = new Student();
                newStudent.name = name + "_server";
                newStudent.age = age + 10;
                return Ok(newStudent);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("file/upload")]
        public IActionResult UploadFile()
        {
            try
            {
                const string UploadDirectory = "uploads"; // 檔案上傳儲存的目錄

                // make sure path exists
                if (Directory.Exists(UploadDirectory) == false) Directory.CreateDirectory(UploadDirectory);

                List<string> fileList = new List<string>();
                List<byte[]> fileBytes = new List<byte[]>();
                List<Task> tasks = new List<Task>();
                // 儲存檔案至指定路徑
                foreach (IFormFile file in Request.Form.Files)
                {
                    if (file == null || file.Length == 0) continue;

                    DateTime curTime = DateTime.Now;
                    string fileName = string.Format("{0}_{1}{2}", System.IO.Path.GetFileNameWithoutExtension(file.Name), curTime.Ticks, System.IO.Path.GetExtension(file.Name));
                    string filePath = Path.Combine(UploadDirectory, fileName);
                    fileList.Add(filePath);
                    tasks.Add(Task.Run(() =>
                    {
                        FileStream stream = new FileStream(filePath, FileMode.Create);
                        // write file content
                        file.CopyTo(stream);
                        stream.Close();
                        stream.Dispose();
                        //// copy wav content
                        //byte[] audioDataWithHeader = new byte[stream.Length];
                        //stream.Seek(0, SeekOrigin.Begin);
                        //stream.Read(audioDataWithHeader, 0, 32);
                        //byte headType = audioDataWithHeader[16];
                        //int headOffset = headType == 0x10 ? 46 : 44;
                        //byte[] audioData = new byte[audioDataWithHeader.Length - headOffset];
                        //Array.Copy(audioDataWithHeader, headOffset, audioData, 0, audioData.Length);
                        //fileBytes.Add(audioData);
                    }));
                }
                if (tasks.Count == 0) throw new Exception("no file uploaded");
                const int timeout = 30 * 60 * 1000; // 30 minutes
                bool bDone = Task.WaitAll(tasks.ToArray(), timeout);
                if (bDone == false) throw new Exception("not all task done...");

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
