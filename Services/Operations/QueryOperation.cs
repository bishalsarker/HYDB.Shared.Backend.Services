using AutoMapper;
using HYDB.Services.Common.Services;
using HYDB.Services.Models;
using HYDB.Services.Repositories;
using HYDB.Services.Services.Common;
using HYDB.Services.Services.Operations;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace HYDB.Services.Services
{
    public class QueryOperation : BaseOperation
    {
        private readonly DataObjectKeyValues _dataObjectKeyValRepo;
        private readonly DataModelProperties _dataModelPropertyRepo;
        private readonly DataObjectResolver _dataObjectResolver;
        private readonly IMapper _mapper;

        public QueryOperation(IConfiguration config, IMapper mapper) : base (config, mapper)
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
            var objects = _dataObjectResolver.ResolveObjects(DataModel, new ConditionalExpression()
            {
                Expression = script.Filter,
                Args = args
            }, !string.IsNullOrEmpty(script.Filter));

            var selectedProperties = GetSelectedProperties(script);

            _responseModel.Data = objects.Select(obj =>
                BuildObject(obj.Id, _dataObjectKeyValRepo.GetDataObjectValues(obj.Id), selectedProperties)
            );
        }

        private List<DataModelProperty> GetSelectedProperties(QueryScript query)
        {
            var allProperties = _dataModelPropertyRepo.GetDataModelProperties(DataModel.Id);
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
            else
            {
                selectedProps = allProperties;
            }

            return selectedProps;
        }

        public object BuildObject(string dataObjId, IEnumerable<DataObjectKeyValue> keyValList, IEnumerable<DataModelProperty> propList)
        {
            IDictionary<string, object> obj = new ExpandoObject();
            obj.Add("id", dataObjId);
            foreach (var keyval in keyValList)
            {
                var prop = propList.ToList().Find(x => x.Id == keyval.KeyString);
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
