namespace Uam.LabHelpDesk.MvcClient.Models.Auth
{
    public class TokenResponseModel
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public string Email { get; set; }
    }
}