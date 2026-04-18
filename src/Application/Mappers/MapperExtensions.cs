using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TiendaUCN.src.Application.Mappers
{
    public class MapperExtensions
    {
        public static void ConfigureMapster(IServiceProvider serviceProvider)
        {
            TypeAdapterConfig.GlobalSettings.Default.IgnoreNullValues(true);
            var userMapper = serviceProvider.GetRequiredService<UserMapper>();
            userMapper.ConfigureAllMappings();
        }

    }
}