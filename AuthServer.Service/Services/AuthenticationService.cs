using AuthServer.Core.Configuration;
using AuthServer.Core.Dtos;
using AuthServer.Core.Models;
using AuthServer.Core.Repositories;
using AuthServer.Core.Services;
using AuthServer.Core.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SharedLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Service.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly List<Client> _clients;
        private readonly ITokenServices _tokenServices;
        private readonly UserManager<UserApp> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<UserRefreshToken> _genericRepository;
        public AuthenticationService(
            IOptions<List<Client>> options,
            ITokenServices tokenServices,
            UserManager<UserApp> userManager,
            IUnitOfWork unitOfWork,
            IGenericRepository<UserRefreshToken> genericRepository)
        {
            _clients = options.Value;
            _tokenServices = tokenServices;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _genericRepository = genericRepository;
        }

        public async Task<Response<TokenDto>> CreateTokenAsync(LoginDto loginDto)
        {
            if (loginDto == null)
            {
                throw new ArgumentNullException(nameof(loginDto));
            }
            UserApp user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                return Response<TokenDto>.Fail("Email or password is wrong", 400, true);
            }
            if (!await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                return Response<TokenDto>.Fail("Email or password is wrong", 400, true);
            }
            TokenDto tokenDto = _tokenServices.CreateToken(user);
            UserRefreshToken userRefreshToken = await _genericRepository.Where(u => u.UserId == user.Id).SingleOrDefaultAsync();
            if (userRefreshToken == null)
            {
                await _genericRepository.AddAsync(new UserRefreshToken { UserId = user.Id, Code = tokenDto.RefreshToken, Expiration = tokenDto.RefreshTokenExpiration });
            }
            else
            {
                userRefreshToken.Code = tokenDto.RefreshToken;
                userRefreshToken.Expiration = tokenDto.RefreshTokenExpiration;
            }
            await _unitOfWork.CommitAsync();
            return Response<TokenDto>.Success(tokenDto, 200);
        }

        public async Task<Response<ClientTokenDto>> CreateTokenByClient(ClientLoginDto clientLoginDto)
        {
            Client client = _clients.SingleOrDefault(c => c.Id == clientLoginDto.ClientId && c.Secret == clientLoginDto.ClientSecret);
            if (client == null)
            {
                return Response<ClientTokenDto>.Fail("ClientId or ClientSecret not found", 404, true);
            }
            ClientTokenDto clientTokenDto = _tokenServices.CreateTokenByClient(client);
            return Response<ClientTokenDto>.Success(clientTokenDto, 200);
        }

        public async Task<Response<TokenDto>> CreateTokenByRefreshTokenAsync(string refreshToken)
        {
            UserRefreshToken userRefreshToken = await _genericRepository.Where(u => u.Code == refreshToken).SingleOrDefaultAsync();
            if (userRefreshToken == null)
            {
                return Response<TokenDto>.Fail("Refresh token not found", 404, true);
            }
            UserApp user = await _userManager.FindByIdAsync(userRefreshToken.UserId);
            if (user == null)
            {
                return Response<TokenDto>.Fail("User not found", 404, true);
            }
            TokenDto tokenDto = _tokenServices.CreateToken(user);
            userRefreshToken.Code = tokenDto.RefreshToken;
            userRefreshToken.Expiration = tokenDto.RefreshTokenExpiration;
            await _unitOfWork.CommitAsync();
            return Response<TokenDto>.Success(tokenDto, 200);
        }

        public async Task<Response<NoDataDto>> RevokeRefreshToken(string refreshToken)
        {
            UserRefreshToken userRefreshToken = await _genericRepository.Where(u => u.Code == refreshToken).SingleOrDefaultAsync();
            if (userRefreshToken == null)
            {
                return Response<NoDataDto>.Fail("Refresh token not found", 404, true);
            }

            _genericRepository.Remove(userRefreshToken);
            await _unitOfWork.CommitAsync();
            return Response<NoDataDto>.Success(204);
        }
    }
}
