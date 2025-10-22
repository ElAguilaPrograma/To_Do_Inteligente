using DB_ToDo;
using DB_ToDo.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using To_Do.Servicios;

namespace To_Do.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ToDoContext _context;
        private readonly IConfiguration _config;
        private readonly IPasswordHasher<Users> _passwordHasher;
        private readonly IServicioUsuarios _servicioUsuarios;

        public UsersController(ToDoContext toDoContext, IConfiguration configuration, IPasswordHasher<Users> passwordHasher, IServicioUsuarios servicioUsuarios)
        {
            _context = toDoContext;
            _config = configuration;
            _passwordHasher = passwordHasher;
            _servicioUsuarios = servicioUsuarios;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            {
                return BadRequest("El email ya está registrado.");
            }

            var user = new Users
            {
                UserName = request.UserName,
                Email = request.Email,
                Password = request.Password, 
                Phone = request.Phone
            };

            //Hashear la contraseña antes de guardarla
            user.Password = _passwordHasher.HashPassword(user, request.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Usuario registrado correctamente"
            });
        }

        // Login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
                return Unauthorized("Email o contraseña incorrectos.");

            var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.Password, request.Password);

            if (verificationResult != PasswordVerificationResult.Success)
                return Unauthorized("Email o contraseña incorrectos.");

            var token = GenerateJwtToken(user);

            return Ok(new { token });
        }

        // Generar JWT
        private string GenerateJwtToken(Users user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("role", "user")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
