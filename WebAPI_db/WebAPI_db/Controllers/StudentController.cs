using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WebAPI_db.Models;
using WebAPI_db.Services;

namespace WebAPI_db.Controllers
{
    [Route("api/student")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        [AllowAnonymous]
        [HttpPost("signup")]
        public IActionResult Signup([FromBody] Student student)
        {
            try
            {
                List<Student> list = StudentService.Instance.FindStudentByName(student.Name);
                if (list.Count > 0) throw new Exception("already exists");

                string? errMsg = StudentService.Instance.Create(student);
                if (errMsg != null) throw new Exception(errMsg);
                return Ok("Ok");
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpGet("all")]
        public IActionResult QueryAll()
        {
            List<Student> students = StudentService.Instance.GetAllStudent();
            return Ok(students);
        }

        [Authorize]
        [HttpGet("info/{id}")]
        public IActionResult QueryById([FromRoute] int id)
        {
            Student? student = StudentService.Instance.FindStudentById(id);
            return Ok(student);
        }

        [Authorize]
        [HttpGet("info")]
        public IActionResult QueryByName([FromQuery] string name)
        {
            List<Student> students = StudentService.Instance.FindStudentByName(name);
            return Ok(students);
        }

        [Authorize]
        [HttpPost("info")]
        public IActionResult Create([FromBody] Student student)
        {
            // 取得 Bearer token
            string token_original = HttpContext.Request.Headers["Authorization"].ToString();
            string token = token_original.Replace("Bearer ", "");
            // 將 token 解析成 JWT token 物件
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken parsedToken = tokenHandler.ReadJwtToken(token);
            // 讀取 token 中的 claims
            Claim? accountClaim = parsedToken.Claims.FirstOrDefault(c => c.Type == "Account");

            UserInfo? userInfo = UserInfoService.Instance.FindByAccount(accountClaim.Value);
            if (userInfo == null) throw new Exception("invalid account");

            string? errMsg = StudentService.Instance.Create(student);
            if (errMsg ==  null)
            {
                return Ok("Ok");
            }
            else
            {
                return BadRequest(errMsg);
            }
        }

        [Authorize]
        [HttpPut("info/{id}")]
        public IActionResult Update([FromRoute] int id, [FromBody] Student student)
        {
            string? errMsg = StudentService.Instance.Update(id, student);
            if (errMsg == null)
            {
                return Ok("Ok");
            }
            else
            {
                return BadRequest(errMsg);
            }
        }

        [Authorize]
        [HttpDelete("info/{id}")]
        public IActionResult Delete([FromRoute] int id)
        {
            string? errMsg = StudentService.Instance.Delete(id);
            if (errMsg == null)
            {
                return Ok("Ok");
            }
            else
            {
                return BadRequest(errMsg);
            }
        }
    }
}
