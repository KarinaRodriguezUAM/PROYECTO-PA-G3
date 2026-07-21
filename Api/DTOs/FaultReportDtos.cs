using System.ComponentModel.DataAnnotations;

namespace Uam.LabHelpDesk.Api.DTOs
{
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

    public class CreateFaultReportDto
    {
        [Required]
        public int EquipmentId { get; set; }

        [Required]
        [StringLength(150)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [RegularExpression("Low|Medium|High|Critical")]
        public string Priority { get; set; } = string.Empty;
    }

    public class UpdateFaultReportDto
    {
        [Required]
        [StringLength(150)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [RegularExpression("Pending|InProgress|Resolved|Closed")]
        public string Status { get; set; } = string.Empty;

        [Required]
        [RegularExpression("Low|Medium|High|Critical")]
        public string Priority { get; set; } = string.Empty;
    }

    public record FaultReportStatusLogDto(
        int Id,
        int FaultReportId,
        int ChangedByUserId,
        string ChangedByUserName,
        string PreviousStatus,
        string NewStatus,
        string? Notes,
        DateTime ChangedAtUtc
    );

    public class UpdateFaultReportStatusDto
    {
        [StringLength(500)]
        public string? Notes { get; set; }
    }
}