namespace Uam.LabHelpDesk.Api.Models;

/// <summary>
/// Entidad de dominio que representa un laboratorio universitario.
/// </summary>
public class Laboratory
{
    /// <summary>
    /// Llave primaria del laboratorio.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nombre único del laboratorio.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Edificio donde se encuentra el laboratorio.
    /// </summary>
    public string Building { get; set; } = string.Empty;

    /// <summary>
    /// Piso del edificio donde está el laboratorio.
    /// </summary>
    public int Floor { get; set; }

    /// <summary>
    /// Capacidad máxima de personas del laboratorio.
    /// </summary>
    public int Capacity { get; set; }

    /// <summary>
    /// Indica si el laboratorio está activo (eliminación lógica).
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
    /// Equipos asociados a este laboratorio.
    /// </summary>
    public ICollection<Equipment> Equipments { get; set; } = new List<Equipment>();
}
