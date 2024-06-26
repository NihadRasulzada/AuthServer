using AutoMapper;
using System;

namespace AuthServer.Service
{
    /// <summary>
    /// Provides a singleton instance of IMapper.
    /// </summary>
    public static class ObjectMapper
    {
        private static readonly Lazy<IMapper> mapper = new Lazy<IMapper>(() =>
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });
            return config.CreateMapper();
        });

        /// <summary>
        /// Gets the IMapper instance.
        /// </summary>
        public static IMapper Mapper => mapper.Value;
    }
}
