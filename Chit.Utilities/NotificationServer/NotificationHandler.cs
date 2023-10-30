

using System.Net;
using Chit.Utilities;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PushNotifications.Server.Google;
using SendGrid;
using SendGrid.Helpers.Mail;
using SmartFormat;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.TwiML.Messaging;
using static Chit.Utilities.Notifications;
using Notification = Chit.Utilities.Notification;

namespace StudiomartBE.NotificationServer;
public class NotificationHandler : INotificationHandler
{
    private readonly NotificationsCollection _notifications;
    private readonly NotificationConfig _appSettings;

    public NotificationHandler(IOptions<NotificationsCollection> notifications, IOptions<NotificationConfig> appSettings)
    {
        _notifications = notifications.Value;
        _appSettings = appSettings.Value;
    }

    Notification GetNotification(string type)
    {
        var jsonString = File.ReadAllText("notifications.json");
        var notifications = JsonConvert.DeserializeObject<NotificationsCollection>(jsonString);
        return notifications.Items.Find(x => x.Type.ToLower() == type.ToLower());
    }

    public void SendNotification(string type, string email = null, string PhoneNumber = null, string UserId = null, dynamic data = null,dynamic[] attachments = null)
    {
        var notification = GetNotification(type);
        if (notification == null)
        {
            throw new Exception("Notification type not found");
        }

        if (notification.IsEmail)
        {
            // Send email
            SendMail(email, notification.Id, data, attachments);
        }

        if (notification.IsSMS)
        {
            // Send SMS
            SendSMS(PhoneNumber, notification.Id, data);
        }

        if (notification.IsPush)
        {
            // Send Push
        }

        if (notification.IsWeb)
        {
            // Send Web
        }

        if (notification.IsMobile)
        {
            // Send Mobile
        }
    }

    public async Task<bool> SendMail(string to, string template, dynamic data = null, dynamic[] attachments = null)
    {
        var mailClient = new SendGridClient(_appSettings.SendGridApiKey);
        var fromEmail = new EmailAddress(_appSettings.SendersEmail, _appSettings.SendersName);
        var toEmail = new EmailAddress(to);
        var readjustedData = data == null ? null : MapTo<dynamic>(data);
        var message = MailHelper.CreateSingleTemplateEmail(fromEmail, toEmail, template, data);
        if(attachments != null)
        {
            foreach (var attachment in attachments)
            {
               try
               {
                await message.AddAttachmentAsync(attachment.filename,attachment.content);
               }
               catch (System.Exception e)
               {
                
                throw;
               }
            }
        }
        var response = await mailClient.SendEmailAsync(message);
        return response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Accepted;
    }

    public async Task<bool> SendSMS(string to, string templateKey, dynamic data = null)
    {
        var accountSID = _appSettings.AccountSID;
        var authToken = _appSettings.AuthToken;
        TwilioClient.Init(accountSID, authToken);
        var message = templateKey;
        if (data != null) message = Smart.Format(message, data);
        var sms = await MessageResource.CreateAsync(to, body: message, from: _appSettings.TwilioPhoneNumber);
        return sms.Status == MessageResource.StatusEnum.Sent;
    }

    public async Task<bool> SendPushNotification(string title, string template, string deviceId, dynamic data = null)
    {
        if (data != null) template = Smart.Format(template, data);
        IFcmClient fcmClient = new FcmClient(new FcmOptions
        {
            ServiceAccountKeyFilePath = "ServiceAccount/firebase.json"
        });
        var fcmRequest = new FcmRequest()
        {
            Message = new PushNotifications.Server.Google.Message
            {
                Token = deviceId,
                Notification = new PushNotifications.Server.Google.Notification
                {
                    Title = title,
                    Body = template,
                }
            },
            ValidateOnly = false,
        };
        var fcmResponse = await fcmClient.SendAsync(fcmRequest);
        return fcmResponse.IsSuccessful;
    }

    private static T MapTo<T>(dynamic data)
    {
        var dataString = JsonConvert.SerializeObject(data);
        return JsonConvert.DeserializeObject<T>(dataString);
    }

}
