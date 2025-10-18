namespace To_Do.Servicios
{
    public interface INotificationService
    {
        void SendReminder(string reminder, int userId);
    }
    public class NotificationService: INotificationService
    {
        public NotificationService()
        {
            
        }

        public void SendReminder(string reminder, int userId)
        {
            Console.WriteLine($"Recodatorio enviado al usuario: {userId}, Mensaje: {reminder}, Hora: {DateTime.Now}");
        }
    }
}
