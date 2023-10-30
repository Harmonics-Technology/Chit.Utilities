namespace Chit.Utilities;

public interface IBaseEmailHandler
{
    Task<bool> SendEmail(string email, string subject, string message, string sendersName);
}
