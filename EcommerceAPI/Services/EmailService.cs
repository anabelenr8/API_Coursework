using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace EcommerceAPI.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;

            // Debugging - Print values to console
            Console.WriteLine($"SMTP Server: {_config["EmailSettings:SmtpServer"]}");
            Console.WriteLine($"SMTP Port: {_config["EmailSettings:SmtpPort"]}");
            Console.WriteLine($"Sender Email: {_config["EmailSettings:SenderEmail"]}");
            Console.WriteLine($"Sender Name: {_config["EmailSettings:SenderName"]}");
            Console.WriteLine($"Password: {_config["EmailSettings:Password"]}");
        }


        public async Task SendEmailAsync(string to, string subject, string body)
        {
            Console.WriteLine($" Attempting to send email to: {to}");

            int smtpPort = int.TryParse(_config["EmailSettings:SmtpPort"], out int port) ? port : 587;
            string smtpServer = _config["EmailSettings:SmtpServer"] ?? "smtp.gmail.com";
            string senderEmail = _config["EmailSettings:SenderEmail"] ?? throw new ArgumentNullException("SenderEmail is missing in configuration.");
            string senderName = _config["EmailSettings:SenderName"] ?? "Ecommerce API";
            string senderPassword = _config["EmailSettings:Password"] ?? throw new ArgumentNullException("Password is missing in configuration.");

            Console.WriteLine($" SMTP Server: {smtpServer}, Port: {smtpPort}, Sender: {senderEmail}");

            var smtpClient = new SmtpClient(smtpServer)
            {
                Port = smtpPort,
                Credentials = new NetworkCredential(senderEmail, senderPassword),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(senderEmail, senderName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            mailMessage.To.Add(to);

            try
            {
                Console.WriteLine($" Sending email...");
                await smtpClient.SendMailAsync(mailMessage);
                Console.WriteLine($" Email sent to {to} successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Failed to send email: {ex.Message}");
            }
        }

    }
}
