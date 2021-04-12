using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TinyBank.Core.Implementation.Data;
using TinyBank.Core.Services;
using TinyBank.Core.Services.Options;
using TinyBank.Web.Extensions;

namespace TinyBank.Web.Controllers
{
    [Route("account")]
    public class AccountController : Controller
    {
        private readonly IAccountService _accounts;
        private readonly ILogger<HomeController> _logger;
        private readonly TinyBankDbContext _dbContext;

        // Path: '/account'
        public AccountController(
            TinyBankDbContext dbContext,
            ILogger<HomeController> logger,
            IAccountService accounts)
        {
            _logger = logger;
            _accounts = accounts;
            _dbContext = dbContext;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("search")]
        public IActionResult Index(string accountId)
        {
            var result = _accounts.GetByAccountId(accountId);

            if (!result.IsSuccessful()) {
                return result.ToActionResult();
            }

            return Json(result.Data);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateAccount(string id,
            [FromBody] UpdateAccountOptions options)
        {
            var result = _accounts.UpdateAccount(id, options);

            if (!result.IsSuccessful()) {
                return result.ToActionResult();
            }

            return Ok();
        }
    }
}
