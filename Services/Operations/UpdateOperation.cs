using AutoMapper;
using HYDB.Services.Models;
using HYDB.Services.Repositories;
using HYDB.Services.Services.Common;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace HYDB.Services.Services.Operations
{
    internal class UpdateOperation : BaseMutationOperation
    {
        private readonly DataObjectKeyValues _dataObjectKeyValRepo;
        private readonly DataModelProperties _dataModelPropertyRepo;
        private readonly DataObjectResolver _dataObjectResolver;
        private readonly IMapper _mapper;

        public UpdateOperation(IConfiguration config, IMapper mapper) : base(config, mapper)
        {
            _dataObjectKeyValRepo = new DataObjectKeyValues(config);
            _dataModelPropertyRepo = new DataModelProperties(config);
            _dataObjectResolver = new DataObjectResolver(
                new DataObjects(config), 
                _dataObjectKeyValRepo, 
                _dataModelPropertyRepo
            );
            _mapper = mapper;
        }

        public override void OnOperationExecution(QueryScript script, IDictionary<string, object> args, IDictionary<string, object> model, string userId)
        {
            UpdateDataObject(_dataObjectResolver.ResolveObjects(DataModel, new ConditionalExpression() {
                Expression = script.Condition,
                Args = args
            }, true), model);
        }

        private void UpdateDataObject(IEnumerable<DataObject> dataObjects, IDictionary<string, object> newDataObject)
        {
            var properties = _dataModelPropertyRepo.GetDataModelProperties(DataModel.Id);
            foreach(var obj in dataObjects)
            {
                foreach (var key in newDataObject.Keys)
                {
                    var prop = properties.Find(prop => prop.Name == key);
                    if(prop != null)
                    {
                        _dataObjectKeyValRepo.UpdateValue(obj.Id, prop.Id, newDataObject[prop.Name].ToString());
                    }
                }
            }
        }
    }
}
