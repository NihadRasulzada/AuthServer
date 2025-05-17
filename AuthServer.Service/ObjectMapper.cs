using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.Internal;

namespace AuthServer.Service
{
    public static class ObjectMapper
    {
        private static readonly Lazy<IMapper> mapper = new Lazy<IMapper>(() =>
        {
            MapperConfiguration mapper = new MapperConfiguration(configuration =>
            {
                configuration.Internal().MethodMappingEnabled = false;
                configuration.AddProfile<DtoMapper>();
            });
            return mapper.CreateMapper();
        });

        public static IMapper Mapper => mapper.Value;
    }
}
