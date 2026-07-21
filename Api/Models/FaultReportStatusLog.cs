using System;

namespace Uam.LabHelpDesk.Api.Models
{
    public class FaultReportStatusLog
    {
        public int Id { get; set; }

        public int FaultReportId { get; set; }

        public int ChangedByUserId { get; set; }

        public string PreviousStatus { get; set; } = string.Empty;

        public string NewStatus { get; set; } = string.Empty;

        public string? Notes { get; set; }

        public DateTime ChangedAtUtc { get; set; }

        // Relaciones
        public FaultReport FaultReport { get; set; } = null!;

        public User ChangedByUser { get; set; } = null!;
    }
}
