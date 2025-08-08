namespace HasatEmlak.Services
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string to, string subject, string body, bool isHtml = true);
        Task<bool> SendContactReplyAsync(string to, string customerName, string originalMessage, string replyMessage);
        Task<bool> SendNewPropertyNotificationAsync(string to, string propertyTitle, string propertyUrl);
    }
}
