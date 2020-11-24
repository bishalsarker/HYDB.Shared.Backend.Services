using AutoMapper;
using HYDB.Services.MapperProfiles;
using HYDB.Services.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace HYDB.Services.Configurations
{
    public static class Initializer
    {
        public static void InitializeHYDBServices(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(UserAccountMap));

            services.AddSingleton<IUserAccountInfo, UserAccountInfo>();
        }
    }
}
