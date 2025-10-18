using DB_ToDo;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.JsonWebTokens;

namespace To_Do.Servicios
{

    public interface IServicioUsuarios
    {
        int ObtenerIdDelUsuario();
        string GetCurrentUserEmail();
    }

    public class ServicioUsuarios: IServicioUsuarios
    {
        private readonly ToDoContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ServicioUsuarios(ToDoContext toDoContext, IHttpContextAccessor httpContextAccessor)
        {
            _context = toDoContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public int ObtenerIdDelUsuario()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var userIdClaim = user?.FindFirst(ClaimTypes.NameIdentifier) ?? user?.FindFirst(JwtRegisteredClaimNames.Sub);

            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out var userId))
            {
                return userId;
            }

            throw new UnauthorizedAccessException("Usuario no autenticado");
        }

        public string GetCurrentUserEmail()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            return user?.FindFirst(ClaimTypes.Email)?.Value ??
                   user?.FindFirst(JwtRegisteredClaimNames.Email)?.Value;
        }
    }
}
