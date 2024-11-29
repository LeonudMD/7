using System;
using System.Threading.Tasks;
using EmailService.Domain.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;

namespace EmailService.Infrastructure.Kafka
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly IConfiguration _configuration;

        public EmailService(ILogger<EmailService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<bool> SendWelcomeEmailAsync(string recipientEmail, string message)
        {
            try
            {
                _logger.LogInformation("Начинаю отправку email.");

                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(_configuration["Smtp:From"]));
                email.To.Add(MailboxAddress.Parse(recipientEmail));
                email.Subject = "Добро пожаловать!";
                email.Body = new TextPart(TextFormat.Html) { Text = message };

                using var smtp = new SmtpClient();

                var host = _configuration["Smtp:Host"];
                var port = int.Parse(_configuration["Smtp:Port"]);
                var username = _configuration["Smtp:Username"];
                var password = _configuration["Smtp:Password"];

                _logger.LogInformation($"Подключение к SMTP-серверу {host}:{port}.");
                await smtp.ConnectAsync(host, port, SecureSocketOptions.StartTls);

                _logger.LogInformation("Аутентификация на SMTP-сервере.");
                await smtp.AuthenticateAsync(username, password);

                _logger.LogInformation("Отправка email...");
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);

                _logger.LogInformation($"Email успешно отправлен на: {recipientEmail}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка при отправке email на: {recipientEmail}. Ошибка: {ex.Message}");
                return false;
            }
        }
    }
}
