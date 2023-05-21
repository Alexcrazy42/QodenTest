using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApp
{
    // TODO 4: unauthorized users should receive 401 status code
    //[Authorize]
    [Route("api/account")]
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [AllowAnonymous]
        [HttpGet]
        public ValueTask<Account> Get()
        {
            /* TODO 3: Get user id from cookie */
            return _accountService.LoadOrCreateAsync(Request.Cookies["userName"]);
        }

        //TODO 5: Endpoint should works only for users with "Admin" Role
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByInternalId([FromRoute] int id)
        {
            var account = _accountService.LoadOrCreateAsync(Request.Cookies["userName"]);
            if (account.Result.Role == "Admin")
            {
                //return _accountService.GetFromCache(id);
                return Ok(_accountService.LoadOrCreateAsync(id));
            }
            else
            {
                return Ok("Only to admin");
            }
            
        }

        [Authorize]
        [HttpPost("counter")]
        public async Task UpdateAccount()
        {
            //Update account in cache, don't bother saving to DB, this is not an objective of this task.
            var account = await Get();
            account.Counter++;
        }
    }
}