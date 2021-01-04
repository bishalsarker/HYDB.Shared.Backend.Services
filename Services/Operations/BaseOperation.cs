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
    public class BaseOperation : IOperation
    {
        protected readonly Response _responseModel;
        private readonly DataModels _dataModelRepo;
        private readonly IMapper _mapper;

        public DataModel DataModel { get; set; }

        public BaseOperation(IConfiguration config, IMapper mapper)
        {
            _dataModelRepo = new DataModels(config);
            _responseModel = new Response() { IsSuccess = false, Message = "Can't resolve data source" };
            _mapper = mapper;
        }

        public Response Run(QueryScript script, IDictionary<string, object> args, IDictionary<string, object> model, string userId)
        {
            DataModel = _dataModelRepo.GetAllDataModelByName(script.DataSource, userId);

            if (DataModel != null)
            {
                OnOperationExecution(script, args, model, userId);

                _responseModel.IsSuccess = true;
                _responseModel.Message = "Operation executed successfully";
            }

            return _responseModel;
        }

        public virtual void OnOperationExecution(QueryScript script, IDictionary<string, object> args, IDictionary<string, object> model, string userId)
        {
            Console.WriteLine("Method is to be defined");
        }
    }
}
