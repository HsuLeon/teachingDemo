using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using WebAPI_put.Models;

namespace WebAPI_put.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PutController : ControllerBase
    {

        [HttpPut("querys")]

        public string Put1([FromQuery] string name, [FromQuery] int age)
        {
            return string.Format("the name is {0}, age is {1}", name, age);
        }

        [HttpPut("route/name/{name}/age/{age}")]
        public string Put2([FromRoute] string name, [FromRoute] int age)
        {
            return string.Format("the name is {0}, age is {1}", name, age);
        }

        [HttpPut("header")]
        public string Put3([FromHeader] string name, [FromHeader] int age)
        {
            return string.Format("the name is {0}, age is {1}", name, age);
        }

        [HttpPut("body/form")]
        public Student Put4([FromForm] Student student)
        {
            string name = student.name;
            int age = student.age;

            Student newStudent = new Student();
            newStudent.name = name + "_server";
            newStudent.age = age + 10;
            return newStudent;
        }

        [HttpPut("body/class")]
        public IActionResult Put5([FromBody] Student student)
        {
            string name = student.name;
            int age = student.age;

            Student newStudent = new Student();
            newStudent.name = name + "_server";
            newStudent.age = age + 10;
            return Ok(newStudent);
        }

        [HttpPost("body/class2")]
        public IActionResult Put6([FromForm] Student student)
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

        [HttpPut("body/json")]
        public IActionResult Put4([FromHeader] string name, [FromHeader] int age, [FromBody] JObject obj)
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
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
