using AuthServer.Core.Dtos;
using AuthServer.Core.Models;
using AutoMapper;

namespace AuthServer.Service
{
    /// <summary>
    /// Defines AutoMapper mapping configuration.
    /// </summary>
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Product, ProductDto>().ReverseMap();
            CreateMap<UserAppDto, UserApp>().ReverseMap();
        }
    }
}
