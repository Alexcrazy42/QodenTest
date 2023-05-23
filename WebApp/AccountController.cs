using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApp
{
    // TODO 4: unauthorized users should receive 401 status code
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/account")]
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }


        
        [HttpGet]
        public ValueTask<Account> Get()
        {
            /* TODO 3: Get user id from cookie */
            return _accountService.LoadOrCreateAsync(Request.Cookies["userName"]);
        }

        //TODO 5: Endpoint should works only for users with "Admin" Role
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByInternalId([FromRoute] int id)
        {
            var account = _accountService.LoadOrCreateAsync(Request.Cookies["userName"]);
            if (account.Result.Role == "Admin")
            {
                return Ok(_accountService.GetFromCache(id));
            }
            else
            {
                return StatusCode(403, "Only to admin");
            }
            
        }

        [HttpPost("counter")]
        public async Task<IActionResult> UpdateAccount()
        {
            //Update account in cache, don't b other saving to DB, this is not an objective of this task.
            var account = await Get();
            account.Counter++;

            return Ok("Count + 1");
        }
    }
}