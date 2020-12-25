using System;
using System.Collections.Generic;
using System.Text;

namespace HYDB.Services.Models
{
    public class Client
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ApiKey { get; set; }
        public string CreatedBy { get; set; }
    }
}
