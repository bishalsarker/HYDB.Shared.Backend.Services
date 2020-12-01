using HYDB.Services.DTO;
using System.Collections.Generic;

namespace HYDB.Services.Services
{
    public interface IDataModelManagement
    {
        Response AddNewDataModel(DataModelPayload newDataModelRequest, string userName);
        Response AddNewProperty(DataModelPropertyPayload newPropertyRequest);
        Response DeleteDataModel(string modelId);
        Response DeleteProperty(string propId);
        Response EditProperty(DataModelPropertyPayload updatePropertyRequest);
        List<DataModelResponse> GetAllDataModel(string userName);
        DataModelResponse GetDataModel(string modelId);
        DataModelPropertyResponse GetProperty(string propertyId);
        Response RenameDataModel(DataModelPayload newDataModelRequest);
    }
}