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
    public class DataServiceManagement : IDataServiceManagement
    {
        private readonly DataServices _dataServiceRepo;
        private readonly ServiceOperations _serviceOperationRepo;
        private readonly UserAccounts _userAccountRepo;
        // private readonly DataObjectKeyValueRepository _dataObjectKeyValueRepo;
        private readonly IMapper _mapper;

        public DataServiceManagement(IConfiguration config, IMapper mapper)
        {
            _dataServiceRepo = new DataServices(config);
            _serviceOperationRepo = new ServiceOperations(config);
            _userAccountRepo = new UserAccounts(config);
            // _dataObjectKeyValueRepo = new DataObjectKeyValueRepository();
            _mapper = mapper;
        }

        public Response AddNewDataService(DataServicePayload newDataServiceRequest, string userName)
        {
            var userModel = _userAccountRepo.GetByUsername(userName).FirstOrDefault();

            if (userModel != null)
            {
                var matchedDataServices = _dataServiceRepo.GetAllDataServiceByName(newDataServiceRequest.Name, userModel.Id);
                if (matchedDataServices != null)
                {
                    return new Response()
                    {
                        IsSuccess = false,
                        Message = "Data service with this name already exists"
                    };
                }
                else
                {
                    return SaveDataServiceToDatabase(newDataServiceRequest, userModel);
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

        private Response SaveDataServiceToDatabase(DataServicePayload newDataServiceRequest, UserAccount userModel)
        {
            DataService newDataService = new DataService()
            {
                Id = Guid.NewGuid().ToString("N").ToUpper(),
                Name = newDataServiceRequest.Name,
                CreatedBy = userModel.Id
            };

            _dataServiceRepo.AddNewDataService(newDataService);

            return new Response()
            {
                IsSuccess = true,
                Message = "Data service created successfully",
                Data = new DataServiceResponse()
                {
                    Id = newDataService.Id,
                    Name = newDataService.Name,
                    Operations = new List<ServiceOperationResponse>()
                }
            };
        }

        public Response RenameDataService(DataServicePayload newDataServiceRequest)
        {
            var matchedDataServices = _dataServiceRepo.GetDataServiceById(newDataServiceRequest.Id);
            if (matchedDataServices != null)
            {
                return UpdateDataService(newDataServiceRequest);
            }
            else
            {
                return new Response()
                {
                    IsSuccess = false,
                    Message = "Data service doesn't exist"
                };
            }
        }

        private Response UpdateDataService(DataServicePayload newDataServiceRequest)
        {
            DataService newDataService = new DataService()
            {
                Id = newDataServiceRequest.Id,
                Name = newDataServiceRequest.Name,
            };

            _dataServiceRepo.UpdateDataService(newDataService);

            return new Response()
            {
                IsSuccess = true,
                Message = "Data service updated successfully",
                Data = new DataServiceResponse()
                {
                    Id = newDataService.Id,
                    Name = newDataService.Name,
                    Operations = GetOperationList(newDataService.Id)
                }
            };
        }

        public DataServiceResponse GetDataService(string modelId)
        {
            var dataService = _dataServiceRepo.GetDataServiceById(modelId);
            var dataServiceResponse = _mapper.Map<DataServiceResponse>(dataService);
            dataServiceResponse.Operations = GetOperationList(dataServiceResponse.Id);

            return dataServiceResponse;
        }

        public List<DataServiceResponse> GetAllDataService(string userName)
        {
            var dataServiceDtoList = new List<DataServiceResponse>();
            var userModel = _userAccountRepo.GetByUsername(userName).FirstOrDefault();

            if (userModel != null)
            {
                foreach (DataService dataService in _dataServiceRepo.GetAllDataServiceByUserId(userModel.Id))
                {
                    var dataServiceResponse = _mapper.Map<DataServiceResponse>(dataService);
                    dataServiceResponse.Operations = GetOperationList(dataServiceResponse.Id);

                    dataServiceDtoList.Add(dataServiceResponse);

                }
            }

            return dataServiceDtoList;
        }

        public Response DeleteDataService(string dataServiceId)
        {
            var operations = _serviceOperationRepo.GetServiceOperations(dataServiceId);
            foreach (var operation in operations)
            {
                _serviceOperationRepo.DeleteServiceOperation(operation.Id);
                // _dataObjectKeyValueRepo.DeleteByKeyString(operation.Id);
            }

            _dataServiceRepo.DeleteDataService(dataServiceId);

            return new Response()
            {
                IsSuccess = true,
                Message = "Data service deleted"
            };
        }

        public Response AddNewOperation(ServiceOperationPayload newOperationRequest)
        {
            var newOperation = _mapper.Map<ServiceOperation>(newOperationRequest);
            newOperation.Id = Guid.NewGuid().ToString("N").ToUpper();
            newOperation.Script = GenerateNewScript();

            var matchedOperation = _serviceOperationRepo.GetServiceOperationByName(newOperation.Name, newOperation.ServiceId);

            if (matchedOperation != null)
            {
                return new Response()
                {
                    IsSuccess = false,
                    Message = "Operation with this name already exists in this data model"
                };
            }
            else
            {
                _serviceOperationRepo.AddNewServiceOperation(newOperation);
                return new Response()
                {
                    IsSuccess = true,
                    Data = _mapper.Map<ServiceOperationResponse>(newOperation),
                    Message = "Operation created successfully"
                };
            }
        }

        private string GenerateNewScript()
        {
            var script = new StringBuilder();
            script.Append("{");
            script.Append(Environment.NewLine);
            script.Append("\t");
            script.Append("\"dataSource\": \"\"");
            script.Append(Environment.NewLine);
            script.Append("}");

            return script.ToString();
        }

        public Response EditOperation(ServiceOperationPayload updateOperationRequest)
        {
            var updatedOperation = _mapper.Map<ServiceOperation>(updateOperationRequest);
            var newlyAddedOperation = _serviceOperationRepo.EditServiceOperationInfo(updatedOperation);
            var response = new Response();

            if (newlyAddedOperation != null)
            {
                response.IsSuccess = true;
                response.Data = newlyAddedOperation;
                response.Message = "Operation updated successfully";
            }
            else
            {
                response.IsSuccess = false;
                response.Message = "Operation can't be updated";
            }

            return response;
        }

        public ServiceOperationResponse GetOperation(string operationId)
        {
            var dataServiceOperation = _serviceOperationRepo.GetServiceOperationById(operationId);
            return _mapper.Map<ServiceOperationResponse>(dataServiceOperation);
        }

        private List<ServiceOperationResponse> GetOperationList(string dataServiceId)
        {

            var dataServiceOperationList = new List<ServiceOperationResponse>();
            foreach (var dataServiceOperation in _serviceOperationRepo.GetServiceOperations(dataServiceId))
            {
                dataServiceOperationList.Add(_mapper.Map<ServiceOperationResponse>(dataServiceOperation));
            };

            return dataServiceOperationList;
        }

        public Response DeleteOperation(string propId)
        {
            _serviceOperationRepo.DeleteServiceOperation(propId);
            // _dataObjectKeyValueRepo.DeleteByKeyString(propId.ToString());

            return new Response()
            {
                IsSuccess = true,
                Message = "Operation deleted"
            };
        }
    }
}
