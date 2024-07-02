using AuthServer.Core.Dtos;
using AuthServer.Core.Models;
using AuthServer.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using SharedLibrary.Dtos;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServer.Service.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<UserApp> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UserService(UserManager<UserApp> userManager, RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }
        public async Task<Response<UserAppDto>> CreateUserAsync(CreateUserDto createUserDto)
        {
            var user = new UserApp
            {
                UserName = createUserDto.UserName,
                Email = createUserDto.Email
            };
            var result = await _userManager.CreateAsync(user, createUserDto.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(x => x.Description).ToList();
                return Response<UserAppDto>.Fail(new ErrorDto(errors, true), 400);
            }
            return Response<UserAppDto>.Success(ObjectMapper.Mapper.Map<UserAppDto>(user), 200);
        }

        public async Task<Response<NoContent>> CreateUserRolesAsync(string userName)
        {
            if(!await _roleManager.RoleExistsAsync("Admin"))
            {
                await _roleManager.CreateAsync(new IdentityRole { Name = "Admin" });
            }
            if (!await _roleManager.RoleExistsAsync("Manager"))
            {
                await _roleManager.CreateAsync(new IdentityRole { Name = "Manager" });
            }

            var user = await _userManager.FindByNameAsync(userName);
            await _userManager.AddToRoleAsync(user, "Manager");
            await _userManager.AddToRoleAsync(user, "Admin");

            return Response<NoContent>.Success(StatusCodes.Status201Created);
        }

        public async Task<Response<UserAppDto>> GetUserByNameAsync(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                return Response<UserAppDto>.Fail("UserName not found", 404, true);
            }
            return Response<UserAppDto>.Success(ObjectMapper.Mapper.Map<UserAppDto>(user), 200);
        }
    }
}
