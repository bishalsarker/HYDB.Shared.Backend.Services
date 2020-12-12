using System;
using System.Collections.Generic;
using System.Text;

namespace HYDB.Services.Models
{
    public class QueryModel
    {
        public string DataSource { get; set; }
        public string[] Fields { get; set; }
        public string Filter { get; set; }
    }
}
