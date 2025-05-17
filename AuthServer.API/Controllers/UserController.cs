using System.Security.Claims;
using AuthServer.API.Controllers.Commons;
using AuthServer.Core.DTOs;
using AuthServer.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthServer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : CustomBaseController
    {
        readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserDto dto) => 
            ActionResultInstance(await _userService.CreateUserAsync(dto));

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetUser()
        {
            var userNameClaim = User.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(userNameClaim))
            {
                return Unauthorized("User is not authenticated.");
            }

            return ActionResultInstance(await _userService.GetUserByNameAsync(userNameClaim));
        }
    }
}
