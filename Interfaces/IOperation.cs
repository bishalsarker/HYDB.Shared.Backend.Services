using HYDB.Services.DTO;
using HYDB.Services.Models;
using System.Collections.Generic;
using System.Dynamic;

namespace HYDB.Services.Interfaces
{
    public interface IOperation
    {
        Response Run(QueryScript script, IDictionary<string, object> args, IDictionary<string, object> model, string userId);
    }
}
