﻿using AuthServer.Core.DTOs;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Dtos;


namespace AuthServer.Core.Services
{
    public interface IUserService
    {
        Task<Response<UserAppDto>> CreateUserAsync(CreateUserDto createUserDto);
        Task<Response<UserAppDto>> GetUserByNameAsync(string userName);
        Task<Response<NoContentResult>> CreateUserRoles(string userName);
    }
}
