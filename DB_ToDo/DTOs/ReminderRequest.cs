using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_ToDo.DTOs
{
    public class ReminderRequest
    {
        public string Date { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
