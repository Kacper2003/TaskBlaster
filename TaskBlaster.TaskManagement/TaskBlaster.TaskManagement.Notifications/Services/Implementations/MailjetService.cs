using Mailjet.Client;
using Mailjet.Client.Resources;
using TaskBlaster.TaskManagement.Notifications.Models;
using TaskBlaster.TaskManagement.Notifications.Services.Interfaces;

namespace TaskBlaster.TaskManagement.Notifications.Services.Implementations;

public class MailjetService : IMailService
{
    private readonly string _apiKey;
    private readonly string _secretKey;
    private readonly string _senderEmail;

    public MailjetService(IConfiguration configuration)
    {
        _apiKey = configuration["Mailjet:ApiKey"] ?? "";
        _secretKey = configuration["Mailjet:SecretKey"] ?? "";
        _senderEmail = configuration["Mailjet:SenderEmail"] ?? "";
    }

    public async Task SendBasicEmailAsync(string to, string subject, string content, EmailContentType contentType)
    {
        var client = new MailjetClient(_apiKey, _secretKey);

        var request = new MailjetRequest
        {
            Resource = Send.Resource
        }
        .Property(Send.FromEmail, _senderEmail)
        .Property(Send.FromName, "Task Blaster")
        .Property(Send.To, to)
        .Property(Send.Subject, subject)
        .Property(Send.HtmlPart, contentType == EmailContentType.Html ? content : null)
        .Property(Send.TextPart, contentType == EmailContentType.Text ? content : null);

        var response = await client.PostAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Detailed Response: {response.GetData()}");

            throw new Exception("Failed to send email.");
        }
    }


    public Task SendTemplateEmailAsync(string to, string subject, int templateId, Dictionary<string, object> variables)
    {
        // Implement template email sending using Mailjet's template feature if needed.
        throw new NotImplementedException();
    }
}
