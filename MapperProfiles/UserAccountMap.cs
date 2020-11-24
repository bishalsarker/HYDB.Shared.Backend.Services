using AutoMapper;
using HYDB.Services.DTO;
using HYDB.Services.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace HYDB.Services.MapperProfiles
{
    class UserAccountMap : Profile
    {
        public UserAccountMap()
        {
            CreateMap<UserAccountPayload, UserAccount>();
        }
    }
}
