using BlogIdp.Models;
using BlogIdp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogIdp.Controllers
{
    [Authorize(Policy = "Permission")]
    [Route("api/user")]
    public class UserController:Controller
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpGet("CreateUser")]
        public async Task<IActionResult> CreateUser(string userName,string passWord)
        {

            var user = await _userService.GetUserAsync(userName);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = userName
                };
                var result = await _userService.CreateUserAsync(user, passWord);
                return Ok();
            }

            return Ok(userName+"已经存在");
        }



    }
}
