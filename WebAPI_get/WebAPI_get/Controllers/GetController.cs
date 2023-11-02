
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace WebAPI_get.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetController : ControllerBase
    {

        [HttpGet("query")]
        public string Get1([FromQuery] string name)
        {
            return string.Format("the name is {0}", name);
        }

        [HttpGet("query2")]
        public string Get2([FromQuery] string name, [FromQuery] int age)
        {
            return string.Format("the name is {0}, age is {1}", name, age);
        }

        [HttpGet("route/{name}")]
        public string Get3([FromRoute] string name)
        {
            return string.Format("the name is {0}", name);
        }

        [HttpGet("route/{name}/{age}")]
        public string Get4([FromRoute] string name, [FromRoute] int age)
        {
            return string.Format("the name is {0}, age is {1}", name, age);
        }

        [HttpGet("route/name/{name}/age/{age}")]
        public string Get5([FromRoute]string name, [FromRoute] int age)
        {
            return string.Format("the name is {0}, age is {1}", name, age);
        }

        [HttpGet("header")]
        public string Get6([FromHeader] string name, [FromHeader] int age)
        {
            return string.Format("the name is {0}, age is {1}", name, age);
        }

        [HttpGet("header2")]
        public string Get7()
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
