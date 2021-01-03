using AutoMapper;
using HYDB.Services.Common.Services;
using HYDB.Services.Models;
using HYDB.Services.Repositories;
using System.Collections.Generic;
using System.Text.Json;

namespace HYDB.Services.Services.Common
{
    internal class DataObjectResolver
    {
        private readonly DataObjects _dataObjRepo;
        private readonly DataObjectKeyValues _dataObjectKeyValRepo;
        private readonly DataModelProperties _dataModelPropertyRepo;
        private readonly IMapper _mapper;

        public DataObjectResolver(DataObjects dataObjRepo, DataObjectKeyValues dataObjectKeyValRepo, DataModelProperties dataModelPropertyRepo)
        {
            _dataObjRepo = dataObjRepo;
            _dataObjectKeyValRepo = dataObjectKeyValRepo;
            _dataModelPropertyRepo = dataModelPropertyRepo;
        }

        public IEnumerable<DataObject> ResolveObjects(DataModel dataModel, ConditionalExpression condition, bool applyFilter)
        {
            var resolvedObjects = new List<DataObject>();
            var dataModelProps = _dataModelPropertyRepo.GetDataModelProperties(dataModel.Id);

            foreach(var obj in _dataObjRepo.GetAllDataObject(dataModel.Id))
            {
                if (applyFilter)
                {
                    bool matches = false;
                    if (!string.IsNullOrEmpty(condition.Expression))
                    {
                        matches = FilterExecutor.Execute(
                        condition.Expression,
                        obj,
                        _dataObjectKeyValRepo.GetDataObjectValues(obj.Id),
                        dataModelProps,
                        GetArgDictionary(condition.Args));
                    }

                    if(matches)
                    {
                        resolvedObjects.Add(obj);
                    }
                }
                else
                {
                    resolvedObjects.Add(obj);
                }
            }

            return resolvedObjects;
        }

        private IDictionary<string, object> GetArgDictionary(IDictionary<string, dynamic> args)
        {
            IDictionary<string, object> argDict = new Dictionary<string, object>();

            if (args != null)
            {
                foreach (var key in args.Keys)
                {
                    argDict.Add(key, GetValueFromJsonElement(args[key]));
                }
            }

            return argDict;
        }

        private dynamic GetValueFromJsonElement(JsonElement value)
        {
            dynamic parsedValue = null;
            switch (value.ValueKind)
            {
                case JsonValueKind.String:
                    parsedValue = value.GetString();
                    break;
                case JsonValueKind.Number:
                    parsedValue = value.GetDouble();
                    break;
            }

            return parsedValue;
        }
    }  
}
