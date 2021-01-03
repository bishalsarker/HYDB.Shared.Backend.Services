using System.Dynamic;

namespace HYDB.Services.DTO
{
    public class OperationRequest
    {
        public string Service { get; set; }
        public string Operation { get; set; }
        public ExpandoObject Args { get; set; }
        public ExpandoObject Model { get; set; }
    }
}
