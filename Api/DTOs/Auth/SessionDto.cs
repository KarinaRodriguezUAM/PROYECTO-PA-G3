namespace Uam.LabHelpDesk.Api.DTOs.Auth;

public record SessionDto
{
    public int Id { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime ExpiresAtUtc { get; set; }

    public bool IsCurrentSession { get; set; }

}