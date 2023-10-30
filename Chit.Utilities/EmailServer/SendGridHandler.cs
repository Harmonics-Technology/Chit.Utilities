﻿using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Chit.Utilities;

public class SendGridHandler : IBaseEmailHandler
{
    private NotificationConfig _globals;
    private readonly ISendGridClient _sendGridClient;
    private readonly ILogger<EmailHandler> _logger;
    public SendGridHandler(NotificationConfig globals, ISendGridClient sendGridClient, ILogger<EmailHandler> logger)
    {
        _globals = globals;
        _sendGridClient = sendGridClient;
        _logger = logger;
    }

    public async Task<bool> SendEmail(string email, string subject, string message, string sendersName)
    {
        try
        {


            var SendGridMessage = new SendGridMessage
            {
                From = new EmailAddress(_globals.SendersEmail, sendersName ?? _globals.SendersName),

                Subject = subject,
            };

            SendGridMessage.AddTo(new EmailAddress(email));

            SendGridMessage.AddContent(MimeType.Html, message);

            var Response = await _sendGridClient.SendEmailAsync(SendGridMessage).ConfigureAwait(false);
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return false;
        }
    }
}
