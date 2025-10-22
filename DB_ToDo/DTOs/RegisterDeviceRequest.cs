using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_ToDo.DTOs
{
    public class RegisterDeviceRequest
    {
        public string DeviceToken { get; set; }
        public string Platform { get; set; }
    }
}
