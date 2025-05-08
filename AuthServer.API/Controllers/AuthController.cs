using AuthServer.API.Controllers.Commons;
using AuthServer.Core.DTOs;
using AuthServer.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace AuthServer.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : CustomBaseController
    {
        readonly IAuthenticationService _authenticationService;

        public AuthController(IAuthenticationService authenticationService) =>
            _authenticationService = authenticationService;


        [HttpPost]
        public async Task<IActionResult> CreateToken(LoginDto loginDto) =>
            ActionResultInstance(await _authenticationService.CreateTokenAsync(loginDto));


        [HttpPost]
        public IActionResult CreateTokenByClient(ClientLoginDto loginDto) =>
            ActionResultInstance(_authenticationService.CreateTokenByClient(loginDto));

        [HttpPost]
        public async Task<IActionResult> RevokeRefreshToken(RefreshTokenDto dto) =>
            ActionResultInstance(await _authenticationService.RevokeRefreshToken(dto.Token));

        [HttpPost]
        public async Task<IActionResult> CreateTokenByRefreshToken(RefreshTokenDto dto) =>
            ActionResultInstance(await _authenticationService.CreateTokenByRefreshToken(dto.Token));
    }
}
