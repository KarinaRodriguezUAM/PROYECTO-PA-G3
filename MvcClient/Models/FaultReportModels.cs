using System.ComponentModel.DataAnnotations;

namespace Uam.LabHelpDesk.MvcClient.Models;

public class FaultReportDto
{
    public int Id { get; set; }

    public int EquipmentId { get; set; }

    public string EquipmentCode { get; set; } = string.Empty;

    public int ReportedByUserId { get; set; }

    public string ReportedBy { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Priority { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public DateTime ReportedAtUtc { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime UpdatedAtUtc { get; set; }

    public int? AssignedToUserId { get; set; }

    public string? AssignedToUser { get; set; }
}

public class FaultReportCreateDto
{
    [Required]
    public int EquipmentId { get; set; }

    [Required]
    [MaxLength(150)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public string Priority { get; set; } = string.Empty;
}

public class FaultReportUpdateDto
{
    [Required]
    [MaxLength(150)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public string Priority { get; set; } = string.Empty;

    [Required]
    public string Status { get; set; } = string.Empty;
}

public class FaultReportStatusLogDto
{
    public int Id { get; set; }
    public int FaultReportId { get; set; }
    public int ChangedByUserId { get; set; }
    public string ChangedByUserName { get; set; } = string.Empty;
    public string PreviousStatus { get; set; } = string.Empty;
    public string NewStatus { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public DateTime ChangedAtUtc { get; set; }
}

public class UpdateFaultReportStatusDto
{
    public string? Notes { get; set; }
}