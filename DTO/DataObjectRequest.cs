using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace HYDB.Services.DTO
{
    public class DataObjectRequest
    {
        public string DataModelId { get; set; }
        public ExpandoObject DataObject { get; set; }
    }
}
