using Employee_Api.ServiceContract;
using Employee_Api.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Employee_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUserService _userService;
        public AccountController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpPost]
        [Route("Authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] Loginviewmodel loginViewModel)
        {
            var user = await _userService.Authenticate(loginViewModel);
            if (user == null) return BadRequest(new { message = "wrong user/pswrd" });
            return Ok(user);
        }
    }
}
