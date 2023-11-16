using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebAPI_db.Models;
using WebAPI_db.Services;
using WebAPI_put.Models;

namespace WebAPI_db.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserInfoController : ControllerBase
    {
        private IConfiguration _config;

        public UserInfoController(IConfiguration config)
        {
            _config = config;
        }

        private string GenerateJSONWebToken(UserInfo userInfo)
        {
            string? token = null;
            try
            {
                if (userInfo == null) throw new Exception("null userInfo");

                string key = _config["Jwt:Key"];
                SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
                SigningCredentials credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                long expiredAt = DateTime.Now.Ticks + 24 * 60 * 60;
                Claim[] claims = new[] {
                    new Claim("Account", userInfo.Account),
                    new Claim("Password", userInfo.Password),
                    new Claim("ExpiredAt", expiredAt.ToString()),
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
                if (req.Account == null) throw new Exception("invalid account");
                if (req.Password == null) throw new Exception("invalid password");

                UserInfo userInfo = UserInfoService.Instance.FindByAccount(req.Account);
                if (userInfo == null || userInfo.Password != req.Password) throw new Exception("invalid account or password");

                string tokenString = GenerateJSONWebToken(userInfo);
                response = Ok(tokenString);
            }
            catch (Exception ex)
            {
                response = BadRequest(new { error = ex.Message });
            }
            return response;
        }

        [AllowAnonymous]
        [HttpPost("signup")]
        public IActionResult Signup([FromBody] UserInfo userInfo)
        {
            IActionResult response;
            try
            {
                if (userInfo.Account == null) throw new Exception("invalid account");
                if (userInfo.Password == null) throw new Exception("invalid password");

                if (userInfo.Name == null || userInfo.Name.Length == 0) userInfo.Name = userInfo.Account;
                userInfo.Gender = 0;
                response = Ok(userInfo);
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
                Claim? accountClaim = parsedToken.Claims.FirstOrDefault(c => c.Type == "Account");
                Claim? passwordClaim = parsedToken.Claims.FirstOrDefault(c => c.Type == "Password");
                Claim? expiredAtClaim = parsedToken.Claims.FirstOrDefault(c => c.Type == "ExpiredAt");
                DateTime expiredTime = new DateTime(long.Parse(expiredAtClaim.Value));

                return Ok(string.Format("account:{0}, password:{1}, expiredAt:{2}", accountClaim?.Value, passwordClaim?.Value, expiredTime));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
