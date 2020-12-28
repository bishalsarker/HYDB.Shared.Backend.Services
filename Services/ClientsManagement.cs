using AutoMapper;
using HYDB.Services.DTO;
using HYDB.Services.Models;
using HYDB.Services.Repositories;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HYDB.Services.Services
{
    public class ClientsManagement
    {
        private readonly Clients _clientsRepo;
        private readonly UserAccounts _userAccounts;
        private readonly IMapper _mapper;

        public ClientsManagement(IConfiguration config, IMapper mapper)
        {
            _clientsRepo = new Clients(config);
            _userAccounts = new UserAccounts(config);
            _mapper = mapper;
        }

        public Response GetAllClients(string userName)
        {
            var clients = new List<ClientResponse>();
            var user = _userAccounts.GetByUsername(userName).FirstOrDefault();
            if(user != null)
            {
                foreach(var client in _clientsRepo.GetAllClients(user.Id))
                {
                    clients.Add(_mapper.Map<ClientResponse>(client));
                }
            }

            return new Response()
            {
                IsSuccess = true,
                Data = clients
            };
            
        }

        public Response AddNewClient(ClientPayload newClient, string userName)
        {
            var user = _userAccounts.GetByUsername(userName).FirstOrDefault();

            if(user != null)
            {
                var matchedClient = _clientsRepo.GetByName(newClient.Name);
                if(matchedClient != null)
                {
                    return new Response()
                    {
                        IsSuccess = false,
                        Message = "Client already exists with this name"
                    };
                }
                else
                {
                    var client = _mapper.Map<Client>(newClient);
                    client.Id = Guid.NewGuid().ToString("N").ToUpper();
                    client.ApiKey = Guid.NewGuid().ToString("N").ToLower();
                    client.CreatedBy = user.Id;
                    _clientsRepo.AddNewClient(client);

                    return new Response()
                    {
                        IsSuccess = true,
                        Message = "Client added successfully",
                        Data = client
                    };
                }
            }
            else
            {
                return new Response()
                {
                    IsSuccess = false,
                    Message = "User can't be resolved"
                };
            }
        }

        public UserAccount GetUserFromApiKey(string apiKey)
        {
            UserAccount user = null;
            var client = _clientsRepo.GetByApiKey(apiKey);
            if(client != null)
            {
                user = _userAccounts.GetById(client.CreatedBy).FirstOrDefault();
            }

            return user;
        }
    }
}
