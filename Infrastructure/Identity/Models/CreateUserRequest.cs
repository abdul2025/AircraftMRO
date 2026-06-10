using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AircraftMRO.Infrastructure.Models
{
    public class CreateUserRequest
    {
        public string FullName { get; set; } = string.Empty;

        public string? EmployeeNumber { get; set; }

        public string Email { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }
}