namespace Uam.LabHelpDesk.MvcClient.Models.Auth;

public class SessionViewModel
{
    public int Id { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime ExpiresAtUtc { get; set; }
    public bool IsCurrentSession { get; set; }
}