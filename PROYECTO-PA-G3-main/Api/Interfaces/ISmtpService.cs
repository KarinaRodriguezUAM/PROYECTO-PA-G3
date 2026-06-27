using System.Threading.Tasks;

namespace Uam.LabHelpDesk.Api.Interfaces
{
    /// <summary>
    /// Servicio para el envío de correos electrónicos.
    /// </summary>
    public interface ISmtpService
    {
        /// <summary>
        /// Envía un correo electrónico de forma asíncrona.
        /// </summary>
        /// <param name="toEmail">Dirección de correo destino.</param>
        /// <param name="subject">Asunto del correo.</param>
        /// <param name="body">Cuerpo del correo (admite formato HTML).</param>
        /// <returns>Verdadero si el envío fue exitoso, falso en caso contrario.</returns>
        Task<bool> SendEmailAsync(string toEmail, string subject, string body);
    }
}
