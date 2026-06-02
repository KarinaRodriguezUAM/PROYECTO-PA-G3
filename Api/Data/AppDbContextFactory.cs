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
<<<<<<< HEAD
            "Server=(localdb)\\MSSQLLocalDB;Database=UamLabHelpDeskDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true");
=======
            "Server=.\\SQLEXPRESS;Database=UamLabHelpDeskDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true");
>>>>>>> 541cdaefe9f9066656415c2607aa24a2e3b3801e
        return new AppDbContext(optionsBuilder.Options);
    }
}
