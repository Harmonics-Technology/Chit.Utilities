namespace Chit.Utilities;

public class NotificationQueue
{
    public string Type { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public dynamic Data { get; set; }
    public dynamic[] Attachments { get; set; }
    public string _typename { get; set; }
}
