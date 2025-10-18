using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_ToDo
{
    public class Tasks
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TaskId { get; set; }  
        public string Title { get; set; } = string.Empty; //string-Empty hace que el campo no pueda estar vacio
        public bool IsCompleted { get; set; }
        public string? Description { get; set; }
        public DateTime? ReminderDate { get; set; }
        public string? HangfireJobId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual Users? Users { get; set; }
    }
}
