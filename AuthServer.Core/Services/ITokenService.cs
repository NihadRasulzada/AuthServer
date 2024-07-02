using AuthServer.Core.Configuration;
using AuthServer.Core.Dtos;
using AuthServer.Core.Models;
using System.Threading.Tasks;

namespace AuthServer.Core.Services
{
    public interface ITokenService
    {
        Task<TokenDto> CreateTokenAsync(UserApp userApp);
        ClientTokenDto CreateTokenByClient(Client client);
    }
}
