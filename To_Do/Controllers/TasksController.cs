using DB_ToDo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace To_Do.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly ToDoContext _context;

        public TasksController(ToDoContext toDoContext)
        {
            _context = toDoContext;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] DB_ToDo.Tasks task)
        {
            _context.Tasks.Add(task); 
            await _context.SaveChangesAsync();
            return Ok(task);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var tasks = await _context.Tasks.ToListAsync();
            return Ok(tasks);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteTask(DB_ToDo.Tasks tasks)
        {
            _context.Tasks.Remove(tasks);
            await _context.SaveChangesAsync();
            return NoContent();
        }

    }
}
