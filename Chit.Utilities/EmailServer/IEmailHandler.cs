namespace Chit.Utilities;

public interface IEmailHandler
{
    Task SendEmail(string email, string subject, string message, string sendersName);

        string ComposeFromTemplate(string name, List<KeyValuePair<string, string>> customValues);
}
