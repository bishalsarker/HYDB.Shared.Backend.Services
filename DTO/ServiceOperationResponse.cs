using System;
using System.Collections.Generic;
using System.Text;

namespace HYDB.Services.DTO
{
    public class ServiceOperationResponse
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Script { get; set; }
    }
}
