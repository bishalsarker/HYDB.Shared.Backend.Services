using HYDB.Services.DTO;
using HYDB.Services.Models;

namespace HYDB.Services.Services
{
    public interface IOperationsManagement
    {
        Client GetClientFromRequest(string apiKey);
        ServiceOperation GetOperationByOpeartionName(string opName, string serviceName, string userId);
        Response RunOperation(OperationRequest opRequest, Client client);
    }
}