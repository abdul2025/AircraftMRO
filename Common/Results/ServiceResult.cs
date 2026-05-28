using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AircraftMRO.Common.Results
{
    public class ServiceResult<T>
    {
        public bool Success { get; set; }

        public string? ErrorMessage { get; set; }

        public T? Data { get; set; }
    }
}