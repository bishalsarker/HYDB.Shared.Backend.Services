using System;
using System.Collections.Generic;
using System.Text;

namespace HYDB.Services.DTO
{
    public class ValidationResponse
    {
        public bool HasError { get; set; }
        public string Message { get; set; }
    }
}
