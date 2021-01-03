using AutoMapper;
using HYDB.Services.Models;
using HYDB.Services.Repositories;
using HYDB.Services.Services.Common;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace HYDB.Services.Services.Operations
{
    internal class DeleteOperation : BaseMutationOperation
    {
        private readonly DataObjects _dataObjRepo;
        private readonly DataObjectKeyValues _dataObjectKeyValRepo;
        private readonly DataObjectResolver _dataObjectResolver;
        private readonly IMapper _mapper;

        public DeleteOperation(IConfiguration config, IMapper mapper) : base(config, mapper)
        {
            _dataObjRepo = new DataObjects(config);
            _dataObjectKeyValRepo = new DataObjectKeyValues(config);
            _dataObjectResolver = new DataObjectResolver(
                _dataObjRepo,
                _dataObjectKeyValRepo,
                new DataModelProperties(config)
            );
            _mapper = mapper;
        }

        public override void OnOperationExecution(QueryScript script, IDictionary<string, object> args, IDictionary<string, object> model, string userId)
        {
            DeleteDataObject(_dataObjectResolver.ResolveObjects(DataModel, new ConditionalExpression()
            {
                Expression = script.Condition,
                Args = args
            }, true));
        }

        private void DeleteDataObject(IEnumerable<DataObject> dataObjects)
        {
            foreach(var obj in dataObjects)
            {
                _dataObjectKeyValRepo.DeleteByObjectId(obj.Id);
                _dataObjRepo.DeleteDataObjectById(obj.Id);
            }
        }
    }
}
