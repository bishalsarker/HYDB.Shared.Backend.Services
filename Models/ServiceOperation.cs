using System;
using System.Collections.Generic;
using System.Text;

namespace HYDB.Services.Models
{
    public class ServiceOperation
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Script { get; set; }
        public string ServiceId { get; set; }
    }
}
