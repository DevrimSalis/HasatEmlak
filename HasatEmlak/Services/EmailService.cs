using System.Net;
using System.Net.Mail;

namespace HasatEmlak.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> SendEmailAsync(string to, string subject, string body, bool isHtml = true)
        {
            try
            {
                var smtpSettings = _configuration.GetSection("EmailSettings");

                using var client = new SmtpClient(smtpSettings["SmtpServer"], int.Parse(smtpSettings["SmtpPort"]))
                {
                    EnableSsl = true,
                    Credentials = new NetworkCredential(smtpSettings["Username"], smtpSettings["Password"])
                };

                var message = new MailMessage
                {
                    From = new MailAddress(smtpSettings["SenderEmail"], smtpSettings["SenderName"]),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = isHtml
                };

                message.To.Add(to);

                await client.SendMailAsync(message);
                _logger.LogInformation($"Email sent successfully to {to}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send email to {to}");
                return false;
            }
        }

        public async Task<bool> SendContactReplyAsync(string to, string customerName, string originalMessage, string replyMessage)
        {
            var subject = "HasatEmlak - Mesajınıza Yanıt";
            var body = GenerateContactReplyTemplate(customerName, originalMessage, replyMessage);

            return await SendEmailAsync(to, subject, body, true);
        }

        public async Task<bool> SendNewPropertyNotificationAsync(string to, string propertyTitle, string propertyUrl)
        {
            var subject = "HasatEmlak - Yeni İlan Eklendi";
            var body = GenerateNewPropertyTemplate(propertyTitle, propertyUrl);

            return await SendEmailAsync(to, subject, body, true);
        }

        private string GenerateContactReplyTemplate(string customerName, string originalMessage, string replyMessage)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: #3498db; color: white; padding: 20px; text-align: center; }}
        .content {{ padding: 20px; background: #f9f9f9; }}
        .footer {{ background: #2c3e50; color: white; padding: 15px; text-align: center; }}
        .original-message {{ background: #e8f4f8; padding: 15px; margin: 15px 0; border-left: 4px solid #3498db; }}
        .reply-message {{ background: white; padding: 15px; margin: 15px 0; border: 1px solid #ddd; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>HasatEmlak</h1>
            <p>Mesajınıza Yanıt</p>
        </div>
        
        <div class='content'>
            <p>Merhaba <strong>{customerName}</strong>,</p>
            <p>Bize gönderdiğiniz mesaja yanıt veriyoruz:</p>
            
            <div class='original-message'>
                <h4>Orijinal Mesajınız:</h4>
                <p>{originalMessage}</p>
            </div>
            
            <div class='reply-message'>
                <h4>Yanıtımız:</h4>
                <p>{replyMessage}</p>
            </div>
            
            <p>Başka sorularınız için bizimle iletişime geçmekten çekinmeyin.</p>
            <p>Saygılarımızla,<br><strong>HasatEmlak Ekibi</strong></p>
        </div>
        
        <div class='footer'>
            <p>&copy; 2024 HasatEmlak. Tüm hakları saklıdır.</p>
            <p>📧 info@hasatemlak.com | 📞 +90 555 090 70 90</p>
        </div>
    </div>
</body>
</html>";
        }

        private string GenerateNewPropertyTemplate(string propertyTitle, string propertyUrl)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: #27ae60; color: white; padding: 20px; text-align: center; }}
        .content {{ padding: 20px; background: #f9f9f9; }}
        .footer {{ background: #2c3e50; color: white; padding: 15px; text-align: center; }}
        .cta-button {{ background: #3498db; color: white; padding: 12px 24px; text-decoration: none; border-radius: 5px; display: inline-block; margin: 15px 0; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>🏠 HasatEmlak</h1>
            <p>Yeni İlan Bildirimi</p>
        </div>
        
        <div class='content'>
            <h2>Yeni İlan Eklendi!</h2>
            <p><strong>{propertyTitle}</strong> adlı yeni bir emlak ilanı sitemize eklendi.</p>
            
            <a href='{propertyUrl}' class='cta-button'>İlanı İncele</a>
            
            <p>Hayalinizdeki evi bulmak için sitemizi ziyaret edin!</p>
            <p>Saygılarımızla,<br><strong>HasatEmlak Ekibi</strong></p>
        </div>
        
        <div class='footer'>
            <p>&copy; 2024 HasatEmlak. Tüm hakları saklıdır.</p>
        </div>
    </div>
</body>
</html>";
        }
    }
}