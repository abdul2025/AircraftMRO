using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AircraftMRO.Infrastructure.Models
{
    public class CreateUserResult
    {
        public string UserId { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;
    }
}