using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Nodes;
using WebAPI_put.Models;

namespace WebAPI_put.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PutController : ControllerBase
    {
        private IConfiguration _config;

        public PutController(IConfiguration config)
        {
            _config = config;
        }

        [AllowAnonymous]
        [HttpPut("querys")]
        public string Put1([FromQuery] string name, [FromQuery] int age)
        {
            return string.Format("the name is {0}, age is {1}", name, age);
        }

        [AllowAnonymous]
        [HttpPut("route/name/{name}/age/{age}")]
        public string Put2([FromRoute] string name, [FromRoute] int age)
        {
            return string.Format("the name is {0}, age is {1}", name, age);
        }

        [AllowAnonymous]
        [HttpPut("header")]
        public string Put3([FromHeader] string name, [FromHeader] int age)
        {
            return string.Format("the name is {0}, age is {1}", name, age);
        }

        [AllowAnonymous]
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

        [AllowAnonymous]
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

        [AllowAnonymous]
        [HttpPut("body/class2")]
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

        [AllowAnonymous]
        [HttpPut("body/json")]
        public IActionResult Put7([FromHeader] string name, [FromHeader] int age, [FromBody] JsonObject obj)
        {
            try
            {
                string nameInObj = obj["name"].GetValue<string>();
                int ageInObj = obj["age"].GetValue<int>();

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

        //======================================== How to set Authentication ==========================================================
        // 1. add package "Microsoft.AspNetCore.Authentication.JwtBearer"
        // 2. add Jwt parametes in "appsettings.json"
        // 3. in project, add file "SwaggerBearerAuthOperationFilter .cs"
        // 4. add codes between line 13 to 42 of Program.cs
        // 5. add "app.UseAuthentication()" in Program.cs
        //=============================================================================================================================

        private string GenerateJSONWebToken(string account, string password)
        {
            string? token = null;
            try
            {
                string key = _config["Jwt:Key"];
                SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
                SigningCredentials credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                Claim[] claims = new[] {
                    new Claim(JwtRegisteredClaimNames.Sub, account),
                    new Claim("Name", account),
                    new Claim("Password", password),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };
                string issuer = _config["Jwt:Issuer"];
                JwtSecurityToken jwtToken = new JwtSecurityToken(
                    issuer,
                    issuer,
                    claims,
                    expires: DateTime.Now.AddHours(12),
                    signingCredentials: credentials);

                JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                token = handler.WriteToken(jwtToken);
            }
            catch (Exception ex)
            {

            }
            return token;
        }

        [AllowAnonymous]
        [HttpPost("signin")]
        public IActionResult Signin([FromBody] LoginRequest req)
        {
            IActionResult response;
            try
            {
                if (req.account == null) throw new Exception("invalid account");
                if (req.password == null) throw new Exception("invalid password");

                string tokenString = GenerateJSONWebToken(req.account, req.password);
                response = Ok(new { token = tokenString });
            }
            catch (Exception ex)
            {
                response = BadRequest(new { error = ex.Message });
            }
            return response;
        }

        [Authorize]
        [HttpPut("verify")]
        public IActionResult VerifyToken()
        {
            try
            {
                // 取得 Bearer token
                string token_original = HttpContext.Request.Headers["Authorization"].ToString();
                string token = token_original.Replace("Bearer ", "");
                // 將 token 解析成 JWT token 物件
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                JwtSecurityToken parsedToken = tokenHandler.ReadJwtToken(token);
                // 讀取 token 中的 claims
                Claim? accountClaim = parsedToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub);
                Claim? nameClaim = parsedToken.Claims.FirstOrDefault(c => c.Type == "Name");
                Claim? passwordClaim = parsedToken.Claims.FirstOrDefault(c => c.Type == "Password");

                return Ok(string.Format("name:{0}, password:{1}", nameClaim?.Value, passwordClaim?.Value));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
