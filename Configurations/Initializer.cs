using AutoMapper;
using HYDB.Services.Interfaces;
using HYDB.Services.MapperProfiles;
using HYDB.Services.Services;
using HYDB.Services.Services.Operations;
using Microsoft.Extensions.DependencyInjection;

namespace HYDB.Services.Configurations
{
    public static class Initializer
    {
        public static void InitializeHYDBServices(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(UserAccountMap), typeof(DataModelMap));

            services.AddScoped<IUserAccountInfo, UserAccountInfo>();
            services.AddScoped<IDataModelManagement, DataModelManagement>();
            services.AddScoped<IDataServiceManagement, DataServiceManagement>();
            services.AddScoped<IOperationFactory, OperationFactory>();
            services.AddScoped<IOperationsManagement, OperationsManagement>();
        }
    }
}
