using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Uam.LabHelpDesk.Api.Interfaces;

namespace Uam.LabHelpDesk.Api.Services
{
    /// <summary>
    /// Servicio para enviar correos electrónicos mediante SMTP.
    /// </summary>
    public class SmtpService : ISmtpService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<SmtpService> _logger;

        public SmtpService(IConfiguration configuration, ILogger<SmtpService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<bool> SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                var smtpSection = _configuration.GetSection("Smtp");
                var host = smtpSection["Host"];
                var portStr = smtpSection["Port"];
                var senderEmail = smtpSection["SenderEmail"];
                var senderName = smtpSection["SenderName"];
                var password = smtpSection["Password"];

                if (password == "pass" || password == "Admin" || senderEmail == "sender@gmail.com")
                {
                    _logger.LogWarning("MODO DESARROLLO: Contraseña SMTP es 'pass' o 'Admin', o remitente es default. Bypasseando envío real de correo. Código enviado a consola.");
                    return true;
                }

                if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(portStr) ||
                    string.IsNullOrWhiteSpace(senderEmail) || string.IsNullOrWhiteSpace(password))
                {
                    _logger.LogError("Configuración SMTP incompleta en appsettings.json.");
                    return false;
                }

                if (!int.TryParse(portStr, out int port))
                {
                    _logger.LogError("El puerto SMTP configurado no es un número válido.");
                    return false;
                }

                using (var client = new SmtpClient(host, port))
                {
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(senderEmail, password);
                    client.EnableSsl = true;

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(senderEmail, senderName ?? "UAM Lab Help Desk"),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true
                    };
                    mailMessage.To.Add(toEmail);

                    await client.SendMailAsync(mailMessage);
                }

                _logger.LogInformation("Correo enviado exitosamente a {Email}", toEmail);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar el correo SMTP a {Email}", toEmail);
                return false;
            }
        }
    }
}
