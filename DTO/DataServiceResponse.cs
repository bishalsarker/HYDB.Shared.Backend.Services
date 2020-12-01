using System;
using System.Collections.Generic;
using System.Text;

namespace HYDB.Services.DTO
{
    public class DataServiceResponse
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<ServiceOperationResponse> Operations { get; set; }
    }
}
