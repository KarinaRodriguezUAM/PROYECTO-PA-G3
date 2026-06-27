namespace Uam.LabHelpDesk.MvcClient.Models.Auth
{
    /// <summary>
    /// Modelo para mapear la respuesta de inicio de sesión de la API (solo SessionToken).
    /// </summary>
    public class LoginResponseModel
    {
        public string SessionToken { get; set; } = string.Empty;
    }
}
