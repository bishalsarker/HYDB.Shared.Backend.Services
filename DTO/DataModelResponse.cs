using System;
using System.Collections.Generic;
using System.Text;

namespace HYDB.Services.DTO
{
    public class DataModelResponse
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<DataModelPropertyResponse> Properties { get; set; }
    }
}
