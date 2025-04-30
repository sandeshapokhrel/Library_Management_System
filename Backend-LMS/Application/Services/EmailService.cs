using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Application.Interfaces;

namespace Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly SmtpClient _smtpClient;
        private readonly string _fromEmail;
        private readonly string _fromName;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _logger = logger;
            var smtpConfig = configuration.GetSection("Smtp");
            
            _logger.LogInformation("Initializing EmailService with SMTP config: {Host}:{Port}",
                smtpConfig["Host"], smtpConfig["Port"]);
            
            _logger.LogInformation("SMTP Configuration: Host={Host}, Port={Port}, Username={Username}, SSL={EnableSsl}",
                smtpConfig["Host"], smtpConfig["Port"], smtpConfig["Username"], smtpConfig["EnableSsl"]);

            _smtpClient = new SmtpClient(smtpConfig["Host"])
            {
                Port = int.Parse(smtpConfig["Port"]),
                Credentials = new NetworkCredential(smtpConfig["Username"], smtpConfig["Password"]),
                EnableSsl = bool.Parse(smtpConfig["EnableSsl"]),
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Timeout = 10000 // 10 second timeout
            };
            _fromEmail = smtpConfig["FromEmail"];
            _fromName = smtpConfig["FromName"];
        }

        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                await _smtpClient.SendMailAsync(
                    new MailMessage(_fromEmail, _fromEmail, "SMTP Test", "Connection test"));
                _logger.LogInformation("SMTP connection test succeeded");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SMTP connection test failed");
                return false;
            }
        }

        public async Task SendTransactionNotificationAsync(
            string studentEmail,
            string adminEmail,
            string transactionType,
            string bookTitle,
            string studentName,
            DateTime? dueDate)
        {
            try
            {
                _logger.LogInformation("Preparing to send {TransactionType} notification to {StudentEmail} and {AdminEmail} for book {BookTitle}",
                    transactionType, studentEmail, adminEmail, bookTitle);
                
                var message = new MailMessage
                {
                    From = new MailAddress(_fromEmail, _fromName),
                    Subject = $"{transactionType}: {bookTitle}",
                    IsBodyHtml = false
                };

                // Set body based on transaction type
                message.Body = transactionType switch
                {
                    "Book Issued" => $"Dear {studentName},\n\nYou have successfully issued the book '{bookTitle}'. Please return it by {dueDate:dd/MM/yyyy}.\n\nThank you!",
                    "Book Returned" => $"Dear {studentName},\n\nYou have successfully returned the book '{bookTitle}'.\n\nThank you!",
                    "Overdue Book" => $"Dear {studentName},\n\nThe book '{bookTitle}' was due on {dueDate:dd/MM/yyyy} and is now overdue. Please return it as soon as possible.\n\nThank you!",
                    _ => $"Dear {studentName},\n\nTransaction {transactionType} completed for book '{bookTitle}'.\n\nThank you!"
                };

                message.To.Add(studentEmail);
                if (!string.IsNullOrEmpty(adminEmail))
                {
                    message.CC.Add(adminEmail);
                }
                
                _logger.LogDebug("Email details - From: {From}, To: {To}, CC: {CC}, Subject: {Subject}",
                    _fromEmail, studentEmail, adminEmail, message.Subject);
                
                await _smtpClient.SendMailAsync(message);
                _logger.LogInformation("Successfully sent {TransactionType} notification to {StudentEmail}",
                    transactionType, studentEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send {TransactionType} notification to {StudentEmail}",
                    transactionType, studentEmail);
                throw;
            }
        }
    }
}