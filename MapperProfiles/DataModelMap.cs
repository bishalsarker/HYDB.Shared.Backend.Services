using AutoMapper;
using HYDB.Services.DTO;
using HYDB.Services.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace HYDB.Services.MapperProfiles
{
    public class DataModelMap : Profile
    {
        public DataModelMap()
        {
            CreateMap<DataModel, DataModelResponse>();

            CreateMap<DataModelPropertyPayload, DataModelProperty>();
            CreateMap<DataModelProperty, DataModelPropertyResponse>();
        }
    }
}
