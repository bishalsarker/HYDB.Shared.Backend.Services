using HYDB.Services.DTO;
using HYDB.Services.Interfaces;
using HYDB.Services.Models;
using HYDB.Services.Repositories;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace HYDB.Services.Services
{
    public class OperationsManagement : IOperationsManagement
    {
        private readonly IOperationFactory _operationFactory;
        private readonly Clients _clientsRepo;
        private readonly DataServices _dataServiceRepo;
        private readonly ServiceOperations _serviceOperationRepo;
        private readonly DataModels _dataModelRepo;

        public OperationsManagement(IOperationFactory operationFactory, IConfiguration config)
        {
            _operationFactory = operationFactory;
            _clientsRepo = new Clients(config);
            _dataServiceRepo = new DataServices(config);
            _serviceOperationRepo = new ServiceOperations(config);
            _dataModelRepo = new DataModels(config);
        }

        public Response RunOperation(OperationRequest opRequest, Client client)
        {
            if (!ValidateOperation(opRequest.Operation, opRequest.Service, client.CreatedBy).HasError)
            {
                var operation = GetOperationByOpeartionName(opRequest.Operation, opRequest.Service, client.CreatedBy);

                if (operation != null)
                {
                    var script = JsonConvert.DeserializeObject<QueryScript>(operation.Script, new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });

                    var resolvedOperation = ResolveOperation(operation, script);
                    return resolvedOperation.Run(script, opRequest.Args, opRequest.Model, client.CreatedBy);
                }
                else
                {
                    return new Response()
                    {
                        IsSuccess = false,
                        Message = "Operation couldn't be resolved",
                    };
                }
            }
            else
            {
                return new Response()
                {
                    IsSuccess = false,
                    Message = "Not a valid operation request"
                };
            }
        }

        private IOperation ResolveOperation(ServiceOperation operation, QueryScript script)
        {
            var operationKey = operation.Type;

            var mutationTypeProp = script.GetType().GetProperty("MutationType");
            if (operationKey == "mutation" && mutationTypeProp != null)
            {
                operationKey += $"_{script.MutationType}";
            }

            return _operationFactory.GetOperation(operationKey);
        }

        public Client GetClientFromRequest(string apiKey)
        {
            Client client = null;
            if (apiKey != null)
            {
                client = _clientsRepo.GetByApiKey(apiKey);
            }

            return client;
        }

        public ValidationResponse ValidateOperation(string opName, string serviceName, string userId)
        {
            var validationResponse = new ValidationResponse()
            {
                HasError = false
            };

            if (GetOperationByOpeartionName(opName, serviceName, userId) == null)
            {
                validationResponse.HasError = true;
                validationResponse.Message = "Operation couldn't be resolved";
            }

            return validationResponse;
        }

        public ServiceOperation GetOperationByOpeartionName(string opName, string serviceName, string userId)
        {
            ServiceOperation operation = null;
            var service = _dataServiceRepo.GetDataServiceByName(serviceName, userId);
            if (service != null)
            {
                operation = _serviceOperationRepo.GetServiceOperationByName(opName, service.Id);
            }

            return operation;
        }

        public DataModel GetDataModelFromOperationDataSource(ServiceOperation operation, string userId)
        {
            DataModel datamodel = null;
            if (operation != null)
            {
                var script = JsonConvert.DeserializeObject<QueryScript>(operation.Script, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });

                if (script != null)
                {
                    datamodel = _dataModelRepo.GetAllDataModelByName(script.DataSource, userId);
                }
            }

            return datamodel;
        }
    }
}
