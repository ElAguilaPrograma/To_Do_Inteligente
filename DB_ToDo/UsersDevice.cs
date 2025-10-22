using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_ToDo
{
    public class UsersDevice
    {
        [Key]
        public int Id { get; set; }
        // FK al usuario existente 
        [Required]
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual Users? Users { get; set; }
        [Required]
        [MaxLength(500)]
        public string DeviceToken { get; set; } // token FCM (registration token / push subscription)
        [MaxLength(50)]
        public string Platform { get; set; } // opcional: "web", "android", "ios"
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastSeen { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
