using System;
using System.Collections.Generic;
using System.Text;

namespace HYDB.Services.Models
{
    public class DataObjectKeyValue
    {
        public string Id { get; set; }
        public string KeyString { get; set; }
        public string Value { get; set; }
        public int DataObjectId { get; set; }
    }
}
