using System.Threading.Tasks;
using Uam.LabHelpDesk.Api.Models;

namespace Uam.LabHelpDesk.Api.Interfaces
{
    /// <summary>
    /// Servicio especializado para el envío de notificaciones automáticas por correo electrónico
    /// sobre eventos importantes en el ciclo de vida de un reporte de avería.
    /// </summary>
    public interface IEmailNotificationService
    {
        /// <summary>
        /// Notifica a todos los técnicos activos que se ha creado un nuevo reporte de avería.
        /// </summary>
        /// <param name="report">El reporte de avería recién creado.</param>
        Task SendReportCreatedAsync(FaultReport report);

        /// <summary>
        /// Notifica al técnico asignado que se le ha asignado la atención de un reporte.
        /// </summary>
        /// <param name="report">El reporte de avería asignado.</param>
        /// <param name="technician">El usuario técnico asignado.</param>
        Task SendReportAssignedAsync(FaultReport report, User technician);

        /// <summary>
        /// Notifica al instructor creador del reporte que el estado ha cambiado (ej. a Resolved).
        /// </summary>
        /// <param name="report">El reporte de avería actualizado.</param>
        /// <param name="newStatus">El nuevo estado asignado al reporte.</param>
        Task SendStatusChangedAsync(FaultReport report, string newStatus);

        /// <summary>
        /// Notifica al instructor creador del reporte que el reporte ha sido cerrado (Closed).
        /// </summary>
        /// <param name="report">El reporte de avería cerrado.</param>
        Task SendReportClosedAsync(FaultReport report);
    }
}
