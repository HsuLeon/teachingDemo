using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System.Text.Json.Nodes;

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

        [HttpPost("body/json")]
        public string Post4([FromHeader] string name, [FromHeader] int age, [FromBody] JsonObject obj)
        {
            return string.Format("the name is {0}, age is {1}, body is {2}", name, age, obj);
        }

        [HttpPost("body/class")]
        public string Post5([FromBody] Student student)
        {
            string name = student.name;
            int age = student.age;

            string str1 = student.ToString();
            string str2 = JsonConvert.SerializeObject(student);
            return string.Format("the name is {0}, age is {1}, str1 is {2}, str2 is {3}", name, age, str1, str2);
        }

        public class Student
        {
            public string name { get; set; }
            public int age { get; set; }
        }

        [HttpPost("body/form")]
        public string Post6([FromForm] Student student)
        {
            string name = student.name;
            int age = student.age;

            string str1 = student.ToString();
            string str2 = JsonConvert.SerializeObject(student);
            return string.Format("the name is {0}, age is {1}, str1 is {2}, str2 is {3}", name, age, str1, str2);
        }
    }
}
