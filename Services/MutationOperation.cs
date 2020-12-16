using AutoMapper;
using HYDB.Services.Models;
using HYDB.Services.Repositories;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace HYDB.Services.Services
{
    public class MutationOperation
    {
        private readonly DataObjects _dataObjRepo;
        private readonly DataObjectKeyValues _dataObjectKeyValRepo;
        private readonly DataModelProperties _dataModelPropertyRepo;
        private readonly IMapper _mapper;

        public MutationOperation(IConfiguration config, IMapper mapper)
        {
            _dataObjRepo = new DataObjects(config);
            _dataObjectKeyValRepo = new DataObjectKeyValues(config);
            _dataModelPropertyRepo = new DataModelProperties(config);
            _mapper = mapper;
        }

        public void AddNewOrUpdateOrDeleteExistingDataObject(ExpandoObject newDataObject, string dataModelId)
        {
            IDictionary<string, object> dataObject = newDataObject;
            if (dataObject.ContainsKey("id"))
            {
                if(dataObject.Keys.Count == 1)
                {
                    DeleteDataObject(dataObject["id"].ToString());
                }
                else
                {
                    UpdateDataObject(newDataObject);
                }
            }
            else
            {
                AddNewDataObject(newDataObject, dataModelId);
            }
        }

        private void AddNewDataObject(ExpandoObject newDataObject, string dataModelId)
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
                    KeyString = _dataModelPropertyRepo.GetDataModelPropertyByName(key, dataModelId).Id,
                    Value = dataObject[key].ToString(),
                    DataObjectId = newDataObj.Id
                });
            }
        }

        private void DeleteDataObject(string dataObjectId)
        {
            _dataObjectKeyValRepo.DeleteByObjectId(dataObjectId);
            _dataObjRepo.DeleteDataObjectById(dataObjectId);
        }

        private void UpdateDataObject(ExpandoObject newDataObject)
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
                        if(dataObject.ContainsKey(prop.Name))
                        {
                            _dataObjectKeyValRepo.UpdateValue(objId, prop.Id, dataObject[prop.Name].ToString());
                        }
                    }
                }
            }
        }
    }
}
