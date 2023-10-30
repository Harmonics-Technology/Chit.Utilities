using static Chit.Utilities.BaseEmailHandler;

namespace Chit.Utilities;

public class NotificationConfig
{
    public string MailGunApiKey { get; set; }
    public string MailGunBaseUrl { get; set; }
    public string MailGunFrom { get; set; }
    public string SendersEmail { get; set; }
    public string SendersName { get; set; }
    public string SendGridApiKey { get; set; }
    public EmailHandlerTypeEnum Handler { get; set; }
    public string AccountSID { get; set; }
    public string AuthToken { get; set; }
    public string TwilioPhoneNumber { get; set; }
}
