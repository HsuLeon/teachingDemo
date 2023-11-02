using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json.Linq;
using WebAPI_delete.Models;

namespace WebAPI_delete.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeleteController : ControllerBase
    {

        [HttpDelete("query")]
        public string Del1([FromQuery] string name)
        {
            return string.Format("the name is {0}", name);
        }

        [HttpDelete("query2")]
        public string Del2([FromQuery] string name, [FromQuery] int age)
        {
            return string.Format("the name is {0}, age is {1}", name, age);
        }

        [HttpDelete("query3")]
        public JObject Del3([FromQuery] string name, [FromQuery] int age)
        {
            JObject obj = new JObject();
            obj["name"] = name;
            obj["age"] = age;
            return obj;
        }

        [HttpDelete("query4")]
        public Student Del4([FromQuery] string name, [FromQuery] int age)
        {
            Student student = new Student();
            student.name = name;
            student.age = age;
            return student;
        }

        [HttpDelete("route/{name}")]
        public string Del5([FromRoute] string name)
        {
            return string.Format("the name is {0}", name);
        }

        [HttpDelete("route/{name}/{age}")]
        public string Del6([FromRoute] string name, [FromRoute] int age)
        {
            return string.Format("the name is {0}, age is {1}", name, age);
        }

        [HttpDelete("route/name/{name}/age/{age}")]
        public string Del7([FromRoute] string name, [FromRoute] int age)
        {
            return string.Format("the name is {0}, age is {1}", name, age);
        }

        [HttpDelete("header")]
        public string Del8([FromHeader] string name, [FromHeader] int age)
        {
            return string.Format("the name is {0}, age is {1}", name, age);
        }

        [HttpDelete("header2")]
        public string Del9()
        {
            string name = GetHeaderValue("name");
            string atrAge = GetHeaderValue("age");
            int age = atrAge != null ? int.Parse(atrAge) : 0;

            return string.Format("the name is {0}, age is {1}", name, age);
        }

        string GetHeaderValue(string key)
        {
            StringValues headerValues;
            return Request.Headers.TryGetValue(key, out headerValues) ? headerValues.First() : null;
        }
    }
}
