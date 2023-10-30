using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using Microsoft.AspNetCore.Hosting;
using static Chit.Utilities.BaseEmailHandler;

namespace Chit.Utilities;

public class EmailHandler : IEmailHandler
{
    private readonly NotificationConfig _globals;
    private readonly IWebHostEnvironment _env;
    private readonly ISendGridClient _sendGridClient;
    private readonly ILogger<EmailHandler> _logger;

    public EmailHandler(IOptions<NotificationConfig> globals, IWebHostEnvironment env, ISendGridClient sendGridClient, ILogger<EmailHandler> logger)
    {
        _globals = globals.Value;
        _env = env;
        _sendGridClient = sendGridClient;
        _logger = logger;
    }

    // This is the code used to pass parameters into the compose method
    //List<KeyValuePair<string, string>> EmailParameters = new List<KeyValuePair<string, string>>();
    //EmailParameters.Add(new KeyValuePair<string, string>("AuthorizationCode", AuthorizationCode.ToUpper()));

    /// <summary>
    /// Send Email Via Send grid Rest API Service
    /// </summary>
    /// <param name="email"></param>
    /// <param name="subject"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public async Task SendEmail(string email, string subject, string message, string SendersName = "")
    {
        try
        {
            var EmailResult = await Build(EmailHandlerTypeEnum.SENDGRID, _globals, _sendGridClient, _logger).SendEmail(email, subject, message, SendersName);
            return;
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            throw;
        }
    }

    public string ComposeFromTemplate(string name, List<KeyValuePair<string, string>> customValues)
    {
        var emailTemplate = string.Empty;


        var emailFolder = Path.Combine(_env.WebRootPath, "EmailTemplates");

        if (!Directory.Exists(emailFolder))
        {
            Directory.CreateDirectory(emailFolder);

        }
        var path = Path.Combine(emailFolder, name ?? "EmailTemplate.html");

        using var TemplateFile = new FileStream(path, FileMode.Open);
        using StreamReader Reader = new StreamReader(TemplateFile);
        emailTemplate = Reader.ReadToEnd();


        if (customValues != null)
        {
            foreach (KeyValuePair<string, string> pair in customValues)
            {
                emailTemplate = emailTemplate.Replace(pair.Key, pair.Value);
            }
        }
        emailTemplate = emailTemplate.Replace("Year", DateTime.Now.Year.ToString());
        return emailTemplate;
    }
}
