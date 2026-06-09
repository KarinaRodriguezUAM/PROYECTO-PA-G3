using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Uam.LabHelpDesk.Api.Data;

/// <summary>
/// Fábrica para crear el contexto en tiempo de diseño (migraciones EF Core).
/// </summary>
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    /// <summary>
    /// Crea una instancia del contexto usando la cadena de conexión por defecto.
    /// </summary>
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlServer(
    "Server=localhost;Database=UamLabHelpDeskDb;User Id=sa;Password=Batman2025;TrustServerCertificate=True");
        return new AppDbContext(optionsBuilder.Options);
    }
}
