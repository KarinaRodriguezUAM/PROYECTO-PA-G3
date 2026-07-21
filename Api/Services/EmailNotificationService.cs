using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Uam.LabHelpDesk.Api.Data;
using Uam.LabHelpDesk.Api.Interfaces;
using Uam.LabHelpDesk.Api.Models;

namespace Uam.LabHelpDesk.Api.Services
{
    /// <summary>
    /// Servicio de notificaciones por correo electrónico desacoplado y localizado mediante archivos .resx.
    /// Reutiliza ISmtpService y la configuración SMTP de appsettings.json.
    /// </summary>
    public class EmailNotificationService : IEmailNotificationService
    {
        private readonly AppDbContext _context;
        private readonly ISmtpService _smtpService;
        private readonly IStringLocalizer<EmailNotificationService> _localizer;
        private readonly ILogger<EmailNotificationService> _logger;

        public EmailNotificationService(
            AppDbContext context,
            ISmtpService smtpService,
            IStringLocalizer<EmailNotificationService> localizer,
            ILogger<EmailNotificationService> logger)
        {
            _context = context;
            _smtpService = smtpService;
            _localizer = localizer;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task SendReportCreatedAsync(FaultReport report)
        {
            try
            {
                var technicians = await _context.Users
                    .Include(u => u.Role)
                    .Where(u => u.IsActive && u.Role != null && u.Role.Name == "Technician")
                    .ToListAsync();

                if (!technicians.Any())
                {
                    _logger.LogWarning("No se encontraron técnicos activos para notificar sobre la creación del reporte {ReportId}.", report.Id);
                    return;
                }

                var (title, equipment, lab, status, dateStr) = ExtractReportDetails(report);

                var subject = _localizer["ReportCreatedSubject"].Value;
                var bodyTemplate = _localizer["ReportCreatedBody"].Value;
                var body = string.Format(bodyTemplate, title, equipment, lab, status, dateStr);

                // Disparar envío en segundo plano para no bloquear el hilo de la solicitud HTTP
                _ = Task.Run(async () =>
                {
                    foreach (var tech in technicians)
                    {
                        if (string.IsNullOrWhiteSpace(tech.Email)) continue;

                        try
                        {
                            bool sent = await _smtpService.SendEmailAsync(tech.Email, subject, body);
                            if (!sent)
                            {
                                _logger.LogWarning("No se pudo enviar notificación de creación de reporte al técnico {Email}.", tech.Email);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error al enviar notificación de reporte creado a {Email}.", tech.Email);
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en SendReportCreatedAsync para el reporte {ReportId}.", report.Id);
            }
        }

        /// <inheritdoc />
        public async Task SendReportAssignedAsync(FaultReport report, User technician)
        {
            try
            {
                if (technician == null || !technician.IsActive || string.IsNullOrWhiteSpace(technician.Email))
                {
                    _logger.LogWarning("El técnico asignado al reporte {ReportId} es nulo o está inactivo. Omitiendo notificación por correo.", report.Id);
                    return;
                }

                var (title, equipment, lab, _, dateStr) = ExtractReportDetails(report);
                var status = "InProgress";

                var subject = _localizer["ReportAssignedSubject"].Value;
                var bodyTemplate = _localizer["ReportAssignedBody"].Value;
                var body = string.Format(bodyTemplate, title, equipment, lab, status, dateStr);
                var email = technician.Email;

                // Disparar envío en segundo plano
                _ = Task.Run(async () =>
                {
                    try
                    {
                        bool sent = await _smtpService.SendEmailAsync(email, subject, body);
                        if (!sent)
                        {
                            _logger.LogWarning("No se pudo enviar notificación de asignación al técnico {Email}.", email);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error al enviar notificación de asignación a {Email}.", email);
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en SendReportAssignedAsync para el reporte {ReportId}.", report.Id);
            }
        }

        /// <inheritdoc />
        public async Task SendStatusChangedAsync(FaultReport report, string newStatus)
        {
            try
            {
                var instructor = report.ReportedByUser ?? await _context.Users.FirstOrDefaultAsync(u => u.Id == report.ReportedByUserId);

                if (instructor == null || !instructor.IsActive || string.IsNullOrWhiteSpace(instructor.Email))
                {
                    _logger.LogWarning("El instructor creador del reporte {ReportId} es nulo o está inactivo. Omitiendo notificación por correo.", report.Id);
                    return;
                }

                var (title, equipment, lab, _, dateStr) = ExtractReportDetails(report);

                var subject = _localizer["StatusChangedSubject"].Value;
                var bodyTemplate = _localizer["StatusChangedBody"].Value;
                var body = string.Format(bodyTemplate, title, equipment, lab, newStatus, dateStr);
                var email = instructor.Email;

                // Disparar envío en segundo plano
                _ = Task.Run(async () =>
                {
                    try
                    {
                        bool sent = await _smtpService.SendEmailAsync(email, subject, body);
                        if (!sent)
                        {
                            _logger.LogWarning("No se pudo enviar notificación de cambio de estado a {Email}.", email);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error al enviar notificación de cambio de estado a {Email}.", email);
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en SendStatusChangedAsync para el reporte {ReportId}.", report.Id);
            }
        }

        /// <inheritdoc />
        public async Task SendReportClosedAsync(FaultReport report)
        {
            try
            {
                var instructor = report.ReportedByUser ?? await _context.Users.FirstOrDefaultAsync(u => u.Id == report.ReportedByUserId);

                if (instructor == null || !instructor.IsActive || string.IsNullOrWhiteSpace(instructor.Email))
                {
                    _logger.LogWarning("El instructor creador del reporte {ReportId} es nulo o está inactivo. Omitiendo notificación por correo.", report.Id);
                    return;
                }

                var (title, equipment, lab, _, dateStr) = ExtractReportDetails(report);
                var status = "Closed";

                var subject = _localizer["ReportClosedSubject"].Value;
                var bodyTemplate = _localizer["ReportClosedBody"].Value;
                var body = string.Format(bodyTemplate, title, equipment, lab, status, dateStr);
                var email = instructor.Email;

                // Disparar envío en segundo plano
                _ = Task.Run(async () =>
                {
                    try
                    {
                        bool sent = await _smtpService.SendEmailAsync(email, subject, body);
                        if (!sent)
                        {
                            _logger.LogWarning("No se pudo enviar notificación de reporte cerrado a {Email}.", email);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error al enviar notificación de reporte cerrado a {Email}.", email);
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en SendReportClosedAsync para el reporte {ReportId}.", report.Id);
            }
        }

        private (string Title, string Equipment, string Laboratory, string Status, string DateStr) ExtractReportDetails(FaultReport report)
        {
            var title = report.Title ?? "Sin título";
            var equipment = report.Equipment != null
                ? $"{report.Equipment.Brand} {report.Equipment.Model} ({report.Equipment.Code})"
                : "No especificado";

            var lab = report.Equipment?.Laboratory != null
                ? report.Equipment.Laboratory.Name
                : "No especificado";

            var status = report.Status ?? "Pendiente";
            var dateStr = DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss UTC");

            return (title, equipment, lab, status, dateStr);
        }
    }
}
