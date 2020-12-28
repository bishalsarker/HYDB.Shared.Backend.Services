using AutoMapper;
using HYDB.Services.DTO;
using HYDB.Services.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace HYDB.Services.MapperProfiles
{
    public class ClientMap : Profile
    {
        public ClientMap()
        {
            CreateMap<ClientPayload, Client>();
            CreateMap<Client, ClientResponse>();
        }
    }
}
