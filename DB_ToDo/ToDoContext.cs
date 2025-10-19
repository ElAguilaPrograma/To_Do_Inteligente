using Microsoft.EntityFrameworkCore;

namespace DB_ToDo
{
    public class ToDoContext: DbContext
    {
        public ToDoContext(DbContextOptions<ToDoContext> options) : base(options)
        {

        }

        public DbSet<Users> Users { get; set; }
        public DbSet<Tasks> Tasks { get; set; }

    }
}
