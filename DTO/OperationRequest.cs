using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace HYDB.Services.DTO
{
    public class OperationRequest
    {
        public string Service { get; set; }
        public string Operation { get; set; }
        public ExpandoObject Args { get; set; }
    }
}
