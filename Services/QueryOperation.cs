using AutoMapper;
using HYDB.Services.Common.Services;
using HYDB.Services.DTO;
using HYDB.Services.Models;
using HYDB.Services.Repositories;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace HYDB.Services.Services
{
    public class QueryOperation
    {
        private readonly DataObjects _dataObjRepo;
        private readonly DataObjectKeyValues _dataObjectKeyValRepo;
        private readonly DataModels _dataModelRepository;
        private readonly DataModelProperties _dataModelPropertyRepo;
        private readonly ServiceOperations _serviceOperationsRepo;
        private readonly DataServices _dataServiceRepo;
        private readonly UserAccounts _userAccountsRepo;
        private readonly IMapper _mapper;

        public QueryOperation(IConfiguration config, IMapper mapper)
        {
            _dataObjRepo = new DataObjects(config);
            _dataObjectKeyValRepo = new DataObjectKeyValues(config);
            _dataModelRepository = new DataModels(config);
            _dataModelPropertyRepo = new DataModelProperties(config);
            _serviceOperationsRepo = new ServiceOperations(config);
            _dataServiceRepo = new DataServices(config);
            _userAccountsRepo = new UserAccounts(config);
            _mapper = mapper;
        }

        public Response Query(string opName, string serviceName, string userName, string args)
        {
            var user = _userAccountsRepo.GetByUsername(userName).FirstOrDefault();

            if(user != null)
            {
                var operation = ResolveOperation(opName, serviceName, user.Id);
                object parsedObject = null;
                if (args != null)
                {
                    parsedObject = ParseJsonToObject(args);
                }

                if (operation != null)
                {
                    var script = JsonConvert.DeserializeObject<QueryScript>(operation.Script, new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });
                    return Execute(script, user.Id, parsedObject);
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
                    Message = "Something went wrong"
                };
            }
        }

        private ServiceOperation ResolveOperation(string opName, string serviceName, string userId)
        {
            var service = _dataServiceRepo.GetDataServiceByName(serviceName, userId);
            var operation = _serviceOperationsRepo.GetServiceOperationByName(opName, service.Id);

            if(service != null && operation != null)
            {
                return operation;
            }
            else
            {
                return null;
            }
        }

        private Response Execute(QueryScript query, string userId, object args)
        {
            var queryResult = new Response() { Data = new List<object>() };
            var dataModel = _dataModelRepository.GetAllDataModelByName(query.DataSource, userId);
            if(dataModel != null)
            {
                var allProperties = ResolveProperties(dataModel);
                var selectedProperties = GetSelectedProperties(query, allProperties);

                if (selectedProperties.Count < 1)
                {
                    selectedProperties = allProperties;
                }

                var dataObjects = GetAllDataObject(dataModel.Id, selectedProperties, query.Filter, args);
                foreach (var obj in dataObjects)
                {
                    queryResult.Data.Add(GetKeyValObject(obj.Id, obj.KeyValueList, selectedProperties));
                }

                queryResult.IsSuccess = true;
            }
            else
            {
                queryResult.IsSuccess = false;
                queryResult.Message = "Data source couldn't be resolved";
            }

            return queryResult;
        }

        private List<DataModelProperty> ResolveProperties(DataModel dataModel)
        {
            var allProperties = _dataModelPropertyRepo.GetDataModelProperties(dataModel.Id);

            if (dataModel != null && allProperties != null)
            {
                return allProperties;
            }
            else
            {
                return new List<DataModelProperty>();
            }
        }

        private List<DataModelProperty> GetSelectedProperties(QueryScript query, List<DataModelProperty> allProperties)
        {
            var selectedProps = new List<DataModelProperty>();
            if (query.Fields != null && query.Fields.Length > 0)
            {
                foreach (var field in query.Fields)
                {
                    var foundProp = allProperties.Find(prop => prop.Name == field);
                    if (foundProp != null)
                    {
                        selectedProps.Add(foundProp);
                    }
                }
            }

            return selectedProps;
        }

        public object ParseJsonToObject(string jsonString)
        {
            IDictionary<string, dynamic> obj = new ExpandoObject();
            JObject jo = JObject.Parse(jsonString);

            foreach (var jProp in jo.Properties())
            {
                var value = Convert.ChangeType(jProp.Value, GetJTokenType(jProp));
                obj.Add(jProp.Name, value);
            }

            dynamic d = obj;
            object parsedObject = (object)d;
            return parsedObject;
        }

        private static Type GetJTokenType(JProperty jProperty)
        {
            var valType = jProperty.Value.Type;
            Type type;

            switch (valType)
            {
                case JTokenType.String:
                    type = typeof(string);
                    break;
                case JTokenType.Integer:
                    type = typeof(double);
                    break;
                case JTokenType.Float:
                    type = typeof(double);
                    break;
                case JTokenType.Boolean:
                    type = typeof(bool);
                    break;
                default:
                    type = typeof(string);
                    break;
            }

            return type;
        }

        public List<DataObject> GetAllDataObject(string referenceId)
        {
            List<DataObject> dataObjList = new List<DataObject>();
            List<DataModelProperty> dataModelProps = _dataModelPropertyRepo.GetDataModelProperties(referenceId);
            foreach (DataObject dob in _dataObjRepo.GetAllDataObject(referenceId))
            {
                DataObject dataObj = new DataObject();

                dataObj.Id = dob.Id;
                dataObj.ReferenceId = dob.ReferenceId;
                dataObj.KeyValueList = _dataObjectKeyValRepo.GetDataObjectValues(dob.Id);

                var keyValueList = dataObj.KeyValueList.ToList();

                foreach (var prop in dataModelProps)
                {
                    var keyValue = keyValueList.Find(x => x.KeyString == prop.Id.ToString());
                    if (keyValue == null)
                    {
                        var newKeyValue = new DataObjectKeyValue()
                        {
                            KeyString = prop.Id.ToString(),
                            Value = "",
                            DataObjectId = dob.Id
                        };
                        _dataObjectKeyValRepo.AddNewDataObjectKeyValue(newKeyValue);

                        dataObj.KeyValueList.Add(newKeyValue);

                    }
                }

                dataObjList.Add(dataObj);

            }

            return dataObjList;
        }

        public List<DataObject> GetAllDataObject(string referenceId, List<DataModelProperty> props, string exp, object args)
        {
            List<DataObject> dataObjList = new List<DataObject>();
            List<DataModelProperty> dataModelProps = _dataModelPropertyRepo.GetDataModelProperties(referenceId);
            foreach (DataObject dob in _dataObjRepo.GetAllDataObject(referenceId))
            {
                DataObject dataObj = new DataObject();

                dataObj.Id = dob.Id;
                dataObj.ReferenceId = dob.ReferenceId;
                dataObj.KeyValueList = _dataObjectKeyValRepo.GetDataObjectValues(dob.Id);

                var keyValueList = dataObj.KeyValueList.ToList();

                bool matches = true;

                if (!string.IsNullOrEmpty(exp))
                {
                    matches = FilterExecutor.Execute(exp, dataObj.Id.ToString(), dataObj.KeyValueList, dataModelProps, args);
                }

                if (matches)
                {
                    foreach (var prop in props)
                    {
                        var keyValue = keyValueList.Find(x => x.KeyString == prop.Id.ToString());

                        if (keyValue == null)
                        {
                            var newKeyValue = new DataObjectKeyValue()
                            {
                                KeyString = prop.Id.ToString(),
                                Value = "",
                                DataObjectId = dob.Id
                            };
                            _dataObjectKeyValRepo.AddNewDataObjectKeyValue(newKeyValue);

                            dataObj.KeyValueList.Add(newKeyValue);

                        }
                    }

                    dataObjList.Add(dataObj);
                }

            }

            return dataObjList;
        }

        public object GetKeyValObject(string dataObjId, List<DataObjectKeyValue> keyValList, List<DataModelProperty> propList)
        {
            IDictionary<string, object> obj = new ExpandoObject();
            obj.Add("id", dataObjId);
            foreach (var keyval in keyValList)
            {
                var prop = propList.Find(x => x.Id == keyval.KeyString);
                if (prop != null)
                {
                    if (!string.IsNullOrWhiteSpace(keyval.Value))
                    {
                        obj.Add(prop.Name, Convert.ChangeType(keyval.Value, PropertyTypeResolver.Resolve(prop.Type)));
                    }
                    else
                    {
                        obj.Add(prop.Name, null);
                    }
                }
            }
            dynamic parsedObject = obj;
            return parsedObject;
        }
    }
}
