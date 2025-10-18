using Azure.Core;
using DB_ToDo;
using DB_ToDo.DTOs;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;
using To_Do.Servicios;

namespace To_Do.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly ToDoContext _context;
        private readonly IServicioUsuarios _servicioUsuarios;
        private readonly ILogger<TasksController> _logger;
        private readonly NlpService _nlpService;
        private readonly INotificationService _notificationService;
        private readonly IBackgroundJobClient _backgroundJobClient;

        public TasksController(ToDoContext toDoContext, 
            IServicioUsuarios servicioUsuarios, 
            ILogger<TasksController> logger, 
            NlpService nlpService,
            INotificationService notificationService,
            IBackgroundJobClient backgroundJobClient)
        {
            _context = toDoContext;
            _servicioUsuarios = servicioUsuarios;
            _logger = logger;
            _nlpService = nlpService;
            _notificationService = notificationService;
            _backgroundJobClient = backgroundJobClient;
        }

        [HttpPost("createtask")]
        public async Task<IActionResult> CreateTask([FromBody] DB_ToDo.Tasks task)
        {
            try
            {
                var userId = _servicioUsuarios.ObtenerIdDelUsuario();
                _logger.LogInformation($"UserId obtenido del token: {userId}");
                Console.WriteLine($"UserId obtenido del token: {userId}");

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                {
                    return Unauthorized(new {message = "Usuario no encontrado o no autorizado"});
                }

                var resultNlp = await _nlpService.AnalyzeTextAsync(task.Title);

                if (resultNlp != null && resultNlp.Reminder_Detected && resultNlp.Full_Date != null)
                {
                    if (DateTime.TryParse(resultNlp.Full_Date, out var reminder_date)){
                        task.ReminderDate = reminder_date;
                    }
                }

                task.UserId = userId;
                task.Users = user;
                task.CreatedAt = DateTime.UtcNow;

                _context.Tasks.Add(task);
                await _context.SaveChangesAsync();

                if (task.ReminderDate.HasValue)
                {
                    string jobId = _backgroundJobClient.Schedule(
                        () => _notificationService.SendReminder(task.Title, task.UserId),
                        task.ReminderDate.Value
                    );

                    task.HangfireJobId = jobId;
                    await _context.SaveChangesAsync();
                }

                return Ok(new
                {
                    message = "Tarea creada correctamente",
                    taskId = task.TaskId,
                    JsonResponse = resultNlp
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new {message = ex.Message});
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al crear la tarea: {ex.Message}");
            }
        }

        [HttpGet("showtasks")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var userId = _servicioUsuarios.ObtenerIdDelUsuario();

                var tasks = await _context.Tasks
                    .Where(t => t.UserId == userId) // Solo las tareas del usuario
                    .ToListAsync();

                return Ok(tasks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener las tareas: {ex.Message}");
            }
        }

        [HttpPut("updatetask/{taskId}")]
        public async Task<IActionResult> UpdateTask(int taskId, [FromBody] TaskUpdateRequest request)
        {
            try
            {
                var userId = _servicioUsuarios.ObtenerIdDelUsuario();

                //Buscar la tarea con el id correcto
                var task = await _context.Tasks.FirstOrDefaultAsync(t => t.TaskId == taskId && t.UserId == userId);

                if (task == null)
                {
                    return NotFound("Tarea no encontrada");
                }

                var resultNlp = await _nlpService.AnalyzeTextAsync(request.Title);

                if (resultNlp != null && resultNlp.Reminder_Detected && resultNlp.Full_Date != null)
                {
                    if (DateTime.TryParse(resultNlp.Full_Date, out var reminder_date)){
                        task.ReminderDate = reminder_date;
                    }
                }

                //Actualizar los campos deseados
                task.Title = request.Title;
                task.Description = request.Description;
                task.IsCompleted = request.IsCompleted;

                //Si hay job en la tarea lo cancelamos
                if (!string.IsNullOrEmpty(task.HangfireJobId))
                {
                    BackgroundJob.Delete(task.HangfireJobId);
                }

                if (task.ReminderDate.HasValue)
                {
                    string newJobId = _backgroundJobClient.Schedule(
                        () => _notificationService.SendReminder(task.Title, task.UserId),
                        task.ReminderDate.Value
                    );
                    task.HangfireJobId = newJobId;
                }

                _context.Update(task);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Tarea actualizada con exito",
                    taskId = task.TaskId
                });
            }

            catch (Exception ex)
            {
                return StatusCode(500, $"Error al actualizar la tarea: {ex.Message}");
            }
        }

        [HttpPatch("completetask/{taskId}")]
        public async Task<IActionResult> CompleteTask(int taskId)
        {
            var userId = _servicioUsuarios.ObtenerIdDelUsuario();

            var task = await _context.Tasks.FirstOrDefaultAsync(t => t.TaskId == taskId && t.UserId == userId);

            task.IsCompleted = !task.IsCompleted;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Tarea completada con exito",
                isCompleted = task.IsCompleted
            });
        }

        [HttpDelete("deletetask/{taskId}")]
        public async Task<IActionResult> DeleteTask(int taskId)
        {
            try
            {
                var userId = _servicioUsuarios.ObtenerIdDelUsuario();

                var task = await _context.Tasks
                    .FirstOrDefaultAsync(t => t.TaskId == taskId && t.UserId == userId);

                if (task == null)
                {
                    return NotFound(new { message="Tarea no encontrada" });
                }

                if (!string.IsNullOrEmpty(task.HangfireJobId))
                {
                    BackgroundJob.Delete(task.HangfireJobId);
                }

                _context.Tasks.Remove(task);
                await _context.SaveChangesAsync();

                return Ok(new {message="Tarea eliminada correctamente"});
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al eliminar la tarea: {ex.Message}");
            }
        }

    }
}
