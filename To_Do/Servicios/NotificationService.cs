using DB_ToDo;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Microsoft.Identity.Client.Platforms.Features.DesktopOs.Kerberos;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace To_Do.Servicios
{
    public interface INotificationService
    {
        //void SendReminder(string reminder, int userId);
        Task SendReminderAsync(string reminder, int userId);
    }
    public class NotificationService: INotificationService
    {
        private readonly ToDoContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<NotificationService> _logger;
        private readonly HttpClient _httpClient;

        public NotificationService(ToDoContext context, IConfiguration configuration, HttpClient httpClient, ILogger<NotificationService> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
            _httpClient = new HttpClient();
        }

        public async Task SendReminderAsync(string reminder, int userId)
        {
            //Console.WriteLine($"Recodatorio enviado al usuario: {userId}, Mensaje: {reminder}, Hora: {DateTime.Now}");
            try
            {
                //Obtener los tokens del usuario actual
                var tokens = await _context.UsersDevices
                    .Where(d => d.UserId == userId && d.IsActive)
                    .Select(d => d.DeviceToken)
                    .ToListAsync();

                if (tokens == null || tokens.Count == 0)
                {
                    _logger.LogInformation($"No se encontraron device tokens para el usuario: {userId}");
                    return;
                }

                // Obtener acceso a token OAuth2 usando las credenciales JSON 
                var serviceAccountPath = _configuration["Firebase:ServiceAccountFilePath"];
                var projectId = _configuration["Firebase:ProjectId"];

                if(string.IsNullOrEmpty(serviceAccountPath) || string.IsNullOrEmpty(projectId))
                {
                    _logger.LogError("Firebase configuration missing (ServiceAccountFilePath / ProjectId).");
                    return;
                }

                GoogleCredential googleCredential;
                using (var stream = System.IO.File.OpenRead(serviceAccountPath))
                {
                    googleCredential = GoogleCredential.FromStream(stream)
                        .CreateScoped("https://www.googleapis.com/auth/firebase.messaging");
                }

                //Obtener el token de acceso
                var accessToken = await googleCredential.UnderlyingCredential.GetAccessTokenForRequestAsync();

                //Para cada token se envia la notificación
                var url = $"https://fcm.googleapis.com/v1/projects/{projectId}/messages:send";

                foreach (var token in tokens)
                {
                    var payload = new
                    {
                        message = new
                        {
                            token = token,
                            notification = new
                            {
                                title = "Recordatorio",
                                body = reminder
                            },
                            data = new
                            {
                                //Si se ocupan aqui los dejo
                                type = "reminder",
                                timestamp = DateTime.UtcNow.ToString("o")
                            },
                            android = new
                            {
                                priority = "HIGH",
                                notification = new
                                {
                                    click_action = "FLUTTER_NOTIFICATION_CLICK"
                                }
                            },
                            webpush = new
                            {
                                headers = new { Urgency = "high" },
                                notification = new
                                {
                                    //Aqui va el icono, las acciones que tomara, etc
                                    title = "Recodatorio",
                                    body = reminder
                                }
                            }
                        }
                    };

                    var json = JsonConvert.SerializeObject(payload);
                    var request = new HttpRequestMessage(HttpMethod.Post, url);
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                    request.Content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await _httpClient.SendAsync(request);

                    if (!response.IsSuccessStatusCode)
                    {
                        var respText = await response.Content.ReadAsStringAsync();
                        _logger.LogError("Error al enviar el FCM: {status} - {resp}", response.StatusCode, respText);
                    }
                    else
                    {
                        var respText = await response.Content.ReadAsStringAsync();
                        _logger.LogInformation("Notificacion enviada correctamente a token. Resp: {resp}", respText);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en SendReminderAsync");
            }
        }

    }
}
