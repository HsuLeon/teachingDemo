using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPI_db.Models;
using WebAPI_db.Services;

namespace WebAPI_db.Controllers
{
    [Route("api/student")]
    [ApiController]
    public class StudentController : ControllerBase
    {

        [Authorize]
        [HttpGet("info/{account}")]
        public IActionResult QueryByAccount([FromRoute] string account)
        {
            Student student = StudentService.Instance.FindStudentByAccount(account);
            return Ok(student);
        }

        [Authorize]
        [HttpGet("info")]
        public IActionResult QueryByName([FromQuery] string name)
        {
            List<Student> students = StudentService.Instance.FindStudentByName(name);
            return Ok(students);
        }
    }
}
