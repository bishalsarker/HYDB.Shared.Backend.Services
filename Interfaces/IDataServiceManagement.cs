using HYDB.Services.DTO;
using System.Collections.Generic;

namespace HYDB.Services.Services
{
    public interface IDataServiceManagement
    {
        Response AddNewDataService(DataServicePayload newDataServiceRequest, string userName);
        Response AddNewOperation(ServiceOperationPayload newOperationRequest);
        Response DeleteDataService(string dataServiceId);
        Response DeleteOperation(string propId);
        Response EditOperation(ServiceOperationPayload updateOperationRequest);
        List<DataServiceResponse> GetAllDataService(string userName);
        DataServiceResponse GetDataService(string modelId);
        ServiceOperationResponse GetOperation(string operationId);
        Response RenameDataService(DataServicePayload newDataServiceRequest);
    }
}