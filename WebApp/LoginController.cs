using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System.Security.Principal;

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
                Response.Cookies.Append("userName", account.ExternalId);
                return Ok("Cookies added");
            }
            //TODO 2: return 404 if user not found
            return NotFound("User with this userName not found");
        }
    }
}