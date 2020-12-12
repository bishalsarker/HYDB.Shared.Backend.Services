using AutoMapper;
using HYDB.Services.Models;
using HYDB.Services.Repositories;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace HYDB.Services.Services
{
    public class DataObjectManagement
    {
        private readonly DataObjects _dataObjRepo;
        private readonly DataObjectKeyValues _dataObjectKeyValRepo;
        private readonly DataModelProperties _dataModelPropertyRepo;
        private readonly IMapper _mapper;

        public DataObjectManagement(IConfiguration config, IMapper mapper)
        {
            _dataObjRepo = new DataObjects(config);
            _dataObjectKeyValRepo = new DataObjectKeyValues(config);
            _dataModelPropertyRepo = new DataModelProperties(config);
            _mapper = mapper;
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
                    obj.Add(prop.Name, keyval.Value);
                }
            }
            dynamic parsedObject = obj;
            return parsedObject;
        }

        public void AddNewOrUpdateExistingDataObject(ExpandoObject newDataObject, string dataModelId)
        {
            IDictionary<string, object> dataObject = newDataObject;
            if (dataObject.ContainsKey("id"))
            {
                UpdateDataObject(newDataObject);
            }
            else
            {
                AddNewDataObject(newDataObject, dataModelId);
            }
        }

        public void AddNewDataObject(ExpandoObject newDataObject, string dataModelId)
        {
            var newDataObj = new DataObject()
            {
                Id = Guid.NewGuid().ToString("N").ToUpper(),
                ReferenceId = dataModelId
            };

            _dataObjRepo.AddNewDataObject(newDataObj);

            IDictionary<string, object> dataObject = newDataObject;

            foreach (var key in dataObject.Keys)
            {
                _dataObjectKeyValRepo.AddNewDataObjectKeyValue(new DataObjectKeyValue()
                {
                    Id = Guid.NewGuid().ToString("N").ToUpper(),
                    KeyString = _dataModelPropertyRepo.GetDataModelPropertyByName(key, dataModelId).Id.ToString(),
                    Value = dataObject[key].ToString(),
                    DataObjectId = newDataObj.Id
                });
            }
        }

        public void DeleteDataObject(string dataObjectId)
        {
            _dataObjectKeyValRepo.DeleteByObjectId(dataObjectId);
            _dataObjRepo.DeleteDataObjectById(dataObjectId);
        }

        public void UpdateDataObject(ExpandoObject newDataObject)
        {
            IDictionary<string, object> dataObject = newDataObject;
            var objId = dataObject["id"].ToString();
            var existingObject = _dataObjectKeyValRepo.GetDataObjectValues(objId);
            if (existingObject != null)
            {
                foreach (var key in existingObject)
                {
                    var prop = _dataModelPropertyRepo.GetDataModelPropertyById(key.KeyString);
                    if (prop != null)
                    {
                        _dataObjectKeyValRepo.UpdateValue(objId, prop.Id, dataObject[prop.Name].ToString());
                    }
                }
            }
        }
    }
}
