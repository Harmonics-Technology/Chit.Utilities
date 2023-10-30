using System.Net;
using RestSharp;
using RestSharp.Authenticators;

namespace Chit.Utilities;

public class MailGunHandler : IBaseEmailHandler
{
    public NotificationConfig _globals;
    public MailGunHandler(NotificationConfig globals)
    {
        _globals = globals;
    }

    public async Task<bool> SendEmail(string email, string subject, string message, string sendersName)
    {
        try
        {
            var restOptions = new RestClientOptions
            {
                BaseUrl = new Uri(_globals.MailGunBaseUrl),
                Authenticator = new HttpBasicAuthenticator("api", _globals.MailGunApiKey)
            };
            var restClient = new RestClient(restOptions);

            var restRequest = new RestRequest
            {
                Resource = "messages"
            };
            restRequest.AddParameter("from", _globals.MailGunFrom);
            restRequest.AddParameter("to", email);
            restRequest.AddParameter("subject", subject);
            restRequest.AddParameter("html", message);
            restRequest.Method = Method.Post;
            var restResponse = restClient.ExecuteAsync(restRequest).Result;

            if (restResponse.StatusCode != HttpStatusCode.OK)
                return false;

            return true;
        }
        catch (System.Exception)
        {
            return false;
        }
    }
}
