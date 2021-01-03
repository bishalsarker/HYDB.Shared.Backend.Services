using AutoMapper;
using HYDB.Services.Models;
using HYDB.Services.Repositories;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace HYDB.Services.Services.Operations
{
    internal class CreateOperation : BaseMutationOperation
    {
        private readonly DataObjects _dataObjRepo;
        private readonly DataObjectKeyValues _dataObjectKeyValRepo;
        private readonly DataModelProperties _dataModelPropertyRepo;
        private readonly IMapper _mapper;

        public CreateOperation(IConfiguration config, IMapper mapper) : base (config, mapper)
        {
            _dataObjRepo = new DataObjects(config);
            _dataObjectKeyValRepo = new DataObjectKeyValues(config);
            _dataModelPropertyRepo = new DataModelProperties(config);
            _mapper = mapper;
        }

        public override void OnOperationExecution(QueryScript script, IDictionary<string, object> args, IDictionary<string, object> model, string userId)
        {
            AddNewDataObject(model, DataModel.Id);
        }

        private void AddNewDataObject(IDictionary<string, object> newDataObject, string dataModelId)
        {
            var newDataObj = new DataObject()
            {
                Id = Guid.NewGuid().ToString("N").ToUpper(),
                ReferenceId = dataModelId
            };

            _dataObjRepo.AddNewDataObject(newDataObj);

            foreach (var key in newDataObject.Keys)
            {
                _dataObjectKeyValRepo.AddNewDataObjectKeyValue(new DataObjectKeyValue()
                {
                    KeyString = _dataModelPropertyRepo.GetDataModelPropertyByName(key, dataModelId).Id,
                    Value = newDataObject[key].ToString(),
                    DataObjectId = newDataObj.Id
                });
            }
        }
    }
}
