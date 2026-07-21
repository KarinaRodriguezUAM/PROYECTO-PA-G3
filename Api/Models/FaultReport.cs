using System.ComponentModel.DataAnnotations;

namespace Uam.LabHelpDesk.Api.Models
{
    public class FaultReport
    {
        public int Id { get; set; }

        public int EquipmentId { get; set; }

        public int ReportedByUserId { get; set; }

        [Required]
        [MaxLength(150)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Priority { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Pending";

        public DateTime ReportedAtUtc { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public DateTime UpdatedAtUtc { get; set; }

        public int? AssignedToUserId { get; set; }

        // Relaciones

        public Equipment Equipment { get; set; } = null!;

        public User ReportedByUser { get; set; } = null!;

        public User? AssignedToUser { get; set; }
    }
}