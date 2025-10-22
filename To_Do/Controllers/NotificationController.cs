using DB_ToDo;
using DB_ToDo.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using To_Do.Servicios;

namespace To_Do.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly IServicioUsuarios _servicioUsuarios;
        private readonly ToDoContext _context;
        private readonly ILogger _logger;

        public NotificationController(IServicioUsuarios servicioUsuarios, ToDoContext context, ILogger<NotificationController> logger)
        {
            _servicioUsuarios = servicioUsuarios;
            _context = context;
            _logger = logger;
        }

        [HttpPost("register-device")]
        public async Task<IActionResult> RegisterDevice([FromBody] RegisterDeviceRequest request)
        {
            var userId = _servicioUsuarios.ObtenerIdDelUsuario();

            // Validar que el userId sea válido
            if (userId <= 0)
            {
                return Unauthorized(new { message = "Usuario no autenticado" });
            }

            // Validar que el request no sea nulo
            if (request == null)
            {
                return BadRequest(new { message = "Request inválido" });
            }

            // Validar que el token no esté vacío
            if (string.IsNullOrEmpty(request.DeviceToken))
            {
                return BadRequest(new { message = "DeviceToken es requerido" });
            }

            var existing = await _context.UsersDevices
                .FirstOrDefaultAsync(d => d.UserId == userId && d.DeviceToken == request.DeviceToken);

            if (existing == null)
            {
                var device = new UsersDevice
                {
                    UserId = userId,
                    DeviceToken = request.DeviceToken,
                    Platform = request.Platform,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };
                _context.UsersDevices.Add(device);
            }
            else
            {
                existing.IsActive = true;
                existing.LastSeen = DateTime.UtcNow;
                _context.UsersDevices.Update(existing);
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Token y plataforma recibidos con exito" });
        }
    }
}
