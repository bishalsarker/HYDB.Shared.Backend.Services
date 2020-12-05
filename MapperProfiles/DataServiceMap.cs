using AutoMapper;
using HYDB.Services.DTO;
using HYDB.Services.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace HYDB.Services.MapperProfiles
{
    public class DataServiceMap : Profile
    {
        public DataServiceMap()
        {
            CreateMap<DataService, DataServiceResponse>();

            CreateMap<ServiceOperationPayload, ServiceOperation>();
            CreateMap<ServiceOperation, ServiceOperationResponse>();
        }
    }
}
