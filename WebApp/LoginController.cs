using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using System.Security.Principal;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace WebApp
{
    [Route("api")]
    public class LoginController : Controller
    {
        private readonly IAccountDatabase _db;
        public IConfiguration _configuration;

        public LoginController(IAccountDatabase db, IConfiguration config)
        {
            _db = db;
            _configuration = config;
        }



        [HttpGet("sign-in")]
        public async Task<IActionResult> Login(string userName)
        {
            var account = await _db.FindByUserNameAsync(userName);
            if (account != null)
            {

                //TODO 1: Generate auth cookie for user 'userName' with external id
                var claims = new Claim[]
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        new Claim("UserName", account.UserName),
                        new Claim("ExternalId", account.ExternalId),
                    };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                    _configuration["Jwt:Issuer"],
                    _configuration["Jwt:Audience"],
                    claims,
                    expires: DateTime.UtcNow.AddMinutes(10),
                    signingCredentials: signIn);

                Response.Cookies.Append("userName", account.ExternalId);
                Response.Cookies.Append("Token", new JwtSecurityTokenHandler().WriteToken(token));


                return Ok("Cookies added");
            }
            //TODO 2: return 404 if user not found
            return NotFound("User with this userName not found");
        }
    }
}