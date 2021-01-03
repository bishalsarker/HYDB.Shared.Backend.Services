using AutoMapper;
using HYDB.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace HYDB.Services.Services.Operations
{
    public class OperationFactory : IOperationFactory
    {
        private readonly IDictionary<string, IOperation> _operations;

        public OperationFactory(IConfiguration config, IMapper mapper)
        {
            _operations = new Dictionary<string, IOperation>()
            {
                { "query", new QueryOperation(config, mapper) },
                { "mutation_create", new CreateOperation(config, mapper) },
                { "mutation_update", new UpdateOperation(config, mapper) },
                { "mutation_delete", new DeleteOperation(config, mapper) },
            };
        }

        public IOperation GetOperation(string opKey)
        {
            return _operations[opKey];
        }
    }
}
