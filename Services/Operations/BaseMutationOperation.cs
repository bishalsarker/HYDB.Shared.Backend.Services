using AutoMapper;
using HYDB.Services.DTO;
using HYDB.Services.Interfaces;
using HYDB.Services.Models;
using HYDB.Services.Repositories;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace HYDB.Services.Services.Operations
{
    internal class BaseMutationOperation : IOperation
    {
        private readonly DataModels _dataModelRepo;
        private readonly IMapper _mapper;

        public DataModel DataModel { get; set; }

        public BaseMutationOperation(IConfiguration config, IMapper mapper)
        {
            _dataModelRepo = new DataModels(config);
            _mapper = mapper;
        }

        public Response Run(QueryScript script, IDictionary<string, object> args, IDictionary<string, object> model, string userId)
        {
            DataModel = _dataModelRepo.GetAllDataModelByName(script.DataSource, userId);
            var response = new Response() { IsSuccess = false, Message = "Can't resolve data source" };

            if (DataModel != null)
            {
                OnOperationExecution(script, args, model, userId);

                response.IsSuccess = true;
                response.Message = "Mutation operation executed successfully";
            }

            return response;
        }

        public virtual void OnOperationExecution(QueryScript script, IDictionary<string, object> args, IDictionary<string, object> model, string userId)
        {
            Console.WriteLine("Method is to be defined");
        }
    }
}
