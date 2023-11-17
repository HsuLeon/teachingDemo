using Microsoft.AspNetCore.Authorization;
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

        private string? GenerateJSONWebToken(UserInfo userInfo)
        {
            string? token = null;
            try
            {
                if (userInfo == null) throw new Exception("null userInfo");

                string key = _config["Jwt:Key"];
                SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
                SigningCredentials credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                DateTime expiredDate = DateTime.Now.AddHours(24);
                long expiredAt = expiredDate.Ticks;
                Claim[] claims = new[] {
                    new Claim("Account", userInfo.Account),
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
                Console.WriteLine(ex.Message);
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

                UserInfo? userInfo = UserInfoService.Instance.FindByAccount(req.Account);
                if (userInfo == null || userInfo.Password != req.Password) throw new Exception("invalid account or password");

                string? tokenString = GenerateJSONWebToken(userInfo);
                if (tokenString == null) throw new Exception("null tokenString");

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
                string? errMsg = UserInfoService.Instance.Create(userInfo.Account, userInfo.Password, userInfo.Name, userInfo.Gender);
                if (errMsg != null) throw new Exception(errMsg);

                response = Ok();
            }
            catch (Exception ex)
            {
                response = BadRequest(new { error = ex.Message });
            }
            return response;
        }

        [Authorize]
        [HttpGet("verify")]
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
                Claim? expiredAtClaim = parsedToken.Claims.FirstOrDefault(c => c.Type == "ExpiredAt");
                DateTime expiredTime = new DateTime(long.Parse(expiredAtClaim.Value));

                return Ok(string.Format("account:{0}, expiredAt:{1}", accountClaim?.Value, expiredTime));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpPut("password")]
        public IActionResult ChangePassword([FromHeader] string Password)
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
                Claim? expiredAtClaim = parsedToken.Claims.FirstOrDefault(c => c.Type == "ExpiredAt");
                DateTime expiredTime = new DateTime(long.Parse(expiredAtClaim.Value));

                TimeSpan ts = DateTime.Now - expiredTime;
                if (ts.TotalSeconds > 0) throw new Exception("token expired, need to re-login");

                UserInfo? userInfo = UserInfoService.Instance.FindByAccount(accountClaim.Value);
                if (userInfo == null) throw new Exception(string.Format("no userInfo for {0}", accountClaim.Value));

                string? errMsg = UserInfoService.Instance.ChangePassword(accountClaim.Value, Password);
                if (errMsg != null) throw new Exception(errMsg);

                return Ok(string.Format("account:{0}, expiredAt:{1}", accountClaim?.Value, expiredTime));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
