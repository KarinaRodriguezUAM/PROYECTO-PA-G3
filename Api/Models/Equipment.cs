namespace Uam.LabHelpDesk.Api.Models;

/// <summary>
/// Entidad de dominio que representa un equipo dentro de un laboratorio.
/// </summary>
public class Equipment
{
    /// <summary>
    /// Llave primaria del equipo.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Identificador del laboratorio al que pertenece el equipo.
    /// </summary>
    public int LaboratoryId { get; set; }

    /// <summary>
    /// Código único del equipo.
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Marca del equipo.
    /// </summary>
    public string Brand { get; set; } = string.Empty;

    /// <summary>
    /// Modelo del equipo.
    /// </summary>
    public string Model { get; set; } = string.Empty;

    /// <summary>
    /// Número de serie único del equipo.
    /// </summary>
    public string SerialNumber { get; set; } = string.Empty;

    /// <summary>
    /// Tipo de equipo: Desktop, Laptop, Printer, Projector, Other.
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Estado del equipo: Operational, UnderRepair, Decommissioned.
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Fecha de compra del equipo (opcional).
    /// </summary>
    public DateOnly? PurchaseDate { get; set; }

    /// <summary>
    /// Indica si el equipo está activo (eliminación lógica).
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Fecha y hora UTC de creación del registro.
    /// </summary>
    public DateTime CreatedAtUtc { get; set; }

    /// <summary>
    /// Fecha y hora UTC de la última actualización del registro.
    /// </summary>
    public DateTime UpdatedAtUtc { get; set; }

    /// <summary>
    /// Laboratorio al que pertenece el equipo (navegación).
    /// </summary>
    public Laboratory? Laboratory { get; set; }

    public ICollection<FaultReport> FaultReports { get; set; } = new List<FaultReport>();


}
