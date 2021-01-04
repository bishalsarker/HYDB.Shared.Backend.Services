using AutoMapper;
using HYDB.Services.DTO;
using HYDB.Services.Models;
using HYDB.Services.Repositories;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HYDB.Services.Services
{
    public class DataModelManagement : IDataModelManagement
    {
        private readonly DataModels _dataModelRepo;
        private readonly DataModelProperties _dataModelPropRepo;
        private readonly UserAccounts _userAccountRepo;
        private readonly DataObjects _dataObjectRepo;
        private readonly DataObjectKeyValues _dataObjectKeyValueRepo;
        private readonly IMapper _mapper;

        public DataModelManagement(IConfiguration config, IMapper mapper)
        {
            _dataModelRepo = new DataModels(config);
            _dataModelPropRepo = new DataModelProperties(config);
            _userAccountRepo = new UserAccounts(config);
            _dataObjectRepo = new DataObjects(config);
            _dataObjectKeyValueRepo = new DataObjectKeyValues(config);
            _mapper = mapper;
        }

        public Response AddNewDataModel(DataModelPayload newDataModelRequest, string userName)
        {
            var userModel = _userAccountRepo.GetByUsername(userName).FirstOrDefault();

            if (userModel != null)
            {
                var matchedDataModels = _dataModelRepo.GetAllDataModelByName(newDataModelRequest.Name, userModel.Id);
                if (matchedDataModels != null)
                {
                    return new Response()
                    {
                        IsSuccess = false,
                        Message = "Data model with this name already exists"
                    };
                }
                else
                {
                    return SaveDataModelToDatabase(newDataModelRequest, userModel);
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

        private Response SaveDataModelToDatabase(DataModelPayload newDataModelRequest, UserAccount userModel)
        {
            DataModel newDataModel = new DataModel()
            {
                Id = Guid.NewGuid().ToString("N").ToUpper(),
                Name = newDataModelRequest.Name,
                CreatedBy = userModel.Id
            };

            _dataModelRepo.AddNewDataModel(newDataModel);

            return new Response()
            {
                IsSuccess = true,
                Message = "Data model created successfully",
                Data = new DataModelResponse()
                {
                    Id = newDataModel.Id,
                    Name = newDataModel.Name,
                    Properties = new List<DataModelPropertyResponse>()
                }
            };
        }

        public Response RenameDataModel(DataModelPayload newDataModelRequest)
        {
            var matchedDataModels = _dataModelRepo.GetDataModelById(newDataModelRequest.Id);
            if (matchedDataModels != null)
            {
                return UpdateDataModel(newDataModelRequest);
            }
            else
            {
                return new Response()
                {
                    IsSuccess = false,
                    Message = "Data model doesn't exist"
                };
            }
        }

        private Response UpdateDataModel(DataModelPayload newDataModelRequest)
        {
            DataModel newDataModel = new DataModel()
            {
                Id = newDataModelRequest.Id,
                Name = newDataModelRequest.Name,
            };

            _dataModelRepo.UpdateDataModel(newDataModel);

            return new Response()
            {
                IsSuccess = true,
                Message = "Data model updated successfully",
                Data = new DataModelResponse()
                {
                    Id = newDataModel.Id,
                    Name = newDataModel.Name,
                    Properties = GetPropertyList(newDataModel.Id)
                }
            };
        }

        public DataModelResponse GetDataModel(string modelId)
        {
            var dataModel = _dataModelRepo.GetDataModelById(modelId);
            var dataModelResponse = _mapper.Map<DataModelResponse>(dataModel);
            dataModelResponse.Properties = GetPropertyList(dataModelResponse.Id);

            return dataModelResponse;
        }

        public List<DataModelResponse> GetAllDataModel(string userName)
        {
            var dataModelDtoList = new List<DataModelResponse>();
            var userModel = _userAccountRepo.GetByUsername(userName).FirstOrDefault();

            if (userModel != null)
            {
                foreach (DataModel dataModel in _dataModelRepo.GetAllDataModelByUserId(userModel.Id))
                {
                    var dataModelResponse = _mapper.Map<DataModelResponse>(dataModel);
                    dataModelResponse.Properties = GetPropertyList(dataModelResponse.Id);

                    dataModelDtoList.Add(dataModelResponse);

                }
            }

            return dataModelDtoList;
        }

        public Response DeleteDataModel(string modelId)
        {
            var properties = _dataModelPropRepo.GetDataModelProperties(modelId);
            foreach (var property in properties)
            {
                _dataModelPropRepo.DeleteDataModelProperty(property.Id);
                _dataObjectKeyValueRepo.DeleteByKeyString(property.Id);
            }

            _dataModelRepo.DeleteDataModel(modelId);

            return new Response()
            {
                IsSuccess = true,
                Message = "Data model deleted"
            };
        }

        public Response AddNewProperty(DataModelPropertyPayload newPropertyRequest)
        {
            var newDataModelProp = _mapper.Map<DataModelProperty>(newPropertyRequest);
            newDataModelProp.Id = Guid.NewGuid().ToString("N").ToUpper();

            var matchedProperty = _dataModelPropRepo.GetDataModelPropertyByName(newDataModelProp.Name, newDataModelProp.DataModelId);

            if (matchedProperty != null)
            {
                return new Response()
                {
                    IsSuccess = false,
                    Message = "Property with this name already exists in this data model"
                };
            }
            else
            {
                _dataModelPropRepo.AddNewDataModelProperty(newDataModelProp);

                var savedObjects = _dataObjectRepo.GetAllDataObject(newDataModelProp.DataModelId);

                foreach(var obj in savedObjects)
                {
                    _dataObjectKeyValueRepo.AddNewDataObjectKeyValue(new DataObjectKeyValue()
                    {
                        KeyString = newDataModelProp.Id,
                        Value = "",
                        DataObjectId = obj.Id
                    });
                }

                return new Response()
                {
                    IsSuccess = true,
                    Data = _mapper.Map<DataModelPropertyResponse>(newDataModelProp),
                    Message = "Property created successfully"
                };
            }
        }

        public Response EditProperty(DataModelPropertyPayload updatePropertyRequest)
        {
            var updatedDataModelProp = _mapper.Map<DataModelProperty>(updatePropertyRequest);
            var newlyAddedProperty = _dataModelPropRepo.EditDataModelProperty(updatedDataModelProp);
            var response = new Response();

            if (newlyAddedProperty != null)
            {
                response.IsSuccess = true;
                response.Data = newlyAddedProperty;
                response.Message = "Property updated successfully";
            }
            else
            {
                response.IsSuccess = false;
                response.Message = "Property can't be updated";
            }

            return response;
        }

        public DataModelPropertyResponse GetProperty(string propertyId)
        {
            var dataModelProperty = _dataModelPropRepo.GetDataModelPropertyById(propertyId);
            return _mapper.Map<DataModelPropertyResponse>(dataModelProperty);
        }

        private List<DataModelPropertyResponse> GetPropertyList(string dataModelId)
        {

            var dataModelPropertyList = new List<DataModelPropertyResponse>();
            foreach (var dataModelProperty in _dataModelPropRepo.GetDataModelProperties(dataModelId))
            {
                dataModelPropertyList.Add(_mapper.Map<DataModelPropertyResponse>(dataModelProperty));
            };

            return dataModelPropertyList;
        }

        public Response DeleteProperty(string propId)
        {
            _dataModelPropRepo.DeleteDataModelProperty(propId);
            _dataObjectKeyValueRepo.DeleteByKeyString(propId);

            return new Response()
            {
                IsSuccess = true,
                Message = "Property deleted with values"
            };
        }
    }
}
