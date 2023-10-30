using System;

namespace Chit.Utilities;
public interface INotificationHandler
{
    public void SendNotification(string type, string email = null, string PhoneNumber = null, string UserId = null, dynamic data = null,dynamic[] attachments = null);
}
