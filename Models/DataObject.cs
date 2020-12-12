using System;
using System.Collections.Generic;
using System.Text;

namespace HYDB.Services.Models
{
    public class DataObject
    {
        public string Id { get; set; }
        public string ReferenceId { get; set; }
        public List<DataObjectKeyValue> KeyValueList { get; set; }
    }
}
