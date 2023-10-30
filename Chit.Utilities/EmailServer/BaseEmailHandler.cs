using Microsoft.Extensions.Logging;
using SendGrid;

namespace Chit.Utilities;

public class BaseEmailHandler
{
    public static IBaseEmailHandler Build(EmailHandlerTypeEnum type, NotificationConfig config, ISendGridClient _sendGridClient, ILogger<EmailHandler> logger)
    {
        switch (type)
        {
            case EmailHandlerTypeEnum.SENDGRID:
                return new SendGridHandler(config, _sendGridClient, logger);

            case EmailHandlerTypeEnum.MAILGUN:
                return new MailGunHandler(config);

            default:
                return new MailGunHandler(config);
        }
    }
    public enum EmailHandlerTypeEnum
    {
        SENDGRID = 1,
        MAILGUN
    }
}
