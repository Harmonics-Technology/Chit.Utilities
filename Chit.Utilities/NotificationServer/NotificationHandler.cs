

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

namespace Chit.Utilities;
public class NotificationHandler : INotificationHandler
{
    private readonly NotificationConfig _appSettings;

    public NotificationHandler(IOptions<NotificationConfig> appSettings)
    {
        Console.WriteLine("NotificationHandler constructor initialized");
        _appSettings = appSettings.Value;
    }

    Notification GetNotification(string type)
    {
        var jsonString = File.ReadAllText("notifications.json");
        var notifications = JsonConvert.DeserializeObject<NotificationsCollection>(jsonString);
        return notifications.Items.Find(x => x.Type.ToLower() == type.ToLower());
    }

    public void SendNotification(string type, string email = null, string PhoneNumber = null, dynamic data = null, dynamic[] attachments = null, Notification thisNotification = null)
    {
        try
        {
            Console.WriteLine("NotificationHandler.SendNotification method called");
            // if(thisNotification == null)
            //   thisNotification = GetNotification(type);
    
            if (thisNotification == null)
            {
                throw new Exception("thisNotification type not found");
            }
    
            if (thisNotification.IsEmail)
            {
                // Send email
                SendMail(email, thisNotification.TemplateId, data, attachments);
            }
    
            if (thisNotification.IsSMS)
            {
                // Send SMS
                SendSMS(PhoneNumber, thisNotification.TemplateId, data);
            }
    
            if (thisNotification.IsPush)
            {
                // Send Push
            }
    
            if (thisNotification.IsWeb)
            {
                // Send Web
            }
    
            if (thisNotification.IsMobile)
            {
                // Send Mobile
            }
        }
        catch (System.Exception ex)
        {
            //log message and stack trace
            Console.WriteLine($"==={ex.Message}===\n{ex.StackTrace}=== handled in NotificationHandler.cs ===");
            throw;
        }
    }

    public async Task<bool> SendMail(string to, string template, dynamic data = null, dynamic[] attachments = null)
    {
        try
        {
            data = MapTo<dynamic>(data);
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
        catch (System.Exception ex)
        {
            //log message and stack trace
            Console.WriteLine($"==={ex.Message}===\n{ex.StackTrace}=== handled in SendMail method ===");
            
            throw;
        }
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
