using DB_ToDo;
using Google.Apis.Auth.OAuth2;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace To_Do.Servicios
{
    public interface INotificationService
    {
        void SendReminder(string reminder, int userId);
    }
    public class NotificationService: INotificationService
    {
        private readonly ToDoContext _context;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly string _projectId;

        public NotificationService(ToDoContext context, IConfiguration configuration, HttpClient httpClient)
        {
            _context = context;
            _configuration = configuration;
            _httpClient = new HttpClient();
        }

        public void SendReminder(string reminder, int userId)
        {
            Console.WriteLine($"Recodatorio enviado al usuario: {userId}, Mensaje: {reminder}, Hora: {DateTime.Now}");
        }

    }
}
