using Microsoft.EntityFrameworkCore;
using Uam.LabHelpDesk.Api.Models;

namespace Uam.LabHelpDesk.Api.Data;

/// <summary>
/// Contexto principal de Entity Framework Core para la API de UAM Lab Help Desk.
/// </summary>
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    /// <summary>
    /// Tabla lógica de laboratorios.
    /// </summary>
    public DbSet<Laboratory> Laboratories => Set<Laboratory>();

    /// <summary>
    /// Tabla lógica de equipos.
    /// </summary>
    public DbSet<Equipment> Equipment => Set<Equipment>();

    /// <summary>
    /// Configuración del modelo de datos (tablas, llaves, índices y restricciones).
    /// </summary>
    /// 
    public DbSet<Role> Roles => Set<Role>();

    public DbSet<User> Users => Set<User>();

    public DbSet<OtpCode> OtpCodes => Set<OtpCode>();




    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Laboratory>(entity =>
        {
            entity.ToTable("Laboratories");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(100).IsRequired();
            entity.HasIndex(x => x.Name).IsUnique();
            entity.Property(x => x.Building).HasMaxLength(50).IsRequired();
            entity.Property(x => x.Floor).IsRequired();
            entity.Property(x => x.Capacity).IsRequired();
            entity.Property(x => x.IsActive).HasDefaultValue(true).IsRequired();
            entity.Property(x => x.CreatedAtUtc).IsRequired();
            entity.Property(x => x.UpdatedAtUtc).IsRequired();
        });

        modelBuilder.Entity<Equipment>(entity =>
        {
            entity.ToTable("Equipment");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Code).HasMaxLength(20).IsRequired();
            entity.HasIndex(x => x.Code).IsUnique();
            entity.Property(x => x.Brand).HasMaxLength(50).IsRequired();
            entity.Property(x => x.Model).HasMaxLength(50).IsRequired();
            entity.Property(x => x.SerialNumber).HasMaxLength(50).IsRequired();
            entity.HasIndex(x => x.SerialNumber).IsUnique();
            entity.Property(x => x.Type).HasMaxLength(30).IsRequired();
            entity.Property(x => x.Status).HasMaxLength(20).IsRequired();
            entity.Property(x => x.PurchaseDate).IsRequired(false);
            entity.Property(x => x.IsActive).HasDefaultValue(true).IsRequired();
            entity.Property(x => x.CreatedAtUtc).IsRequired();
            entity.Property(x => x.UpdatedAtUtc).IsRequired();

            entity.HasOne(x => x.Laboratory)
                  .WithMany(l => l.Equipments)
                  .HasForeignKey(x => x.LaboratoryId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Token)
                  .IsRequired()
                  .HasMaxLength(500);

            entity.HasIndex(e => e.Token).IsUnique();

            entity.HasOne(e => e.User)
                  .WithMany()
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Roles");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Name)
                .HasMaxLength(50)
                .IsRequired();

            entity.HasIndex(x => x.Name)
                .IsUnique();

            entity.Property(x => x.Description)
                .HasMaxLength(200);

            entity.Property(x => x.IsActive)
                .HasDefaultValue(true);

            entity.Property(x => x.CreatedAtUtc)
                .IsRequired();

            entity.Property(x => x.UpdatedAtUtc)
                .IsRequired();
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.FirstName)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.LastName)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.Email)
                .HasMaxLength(200)
                .IsRequired();

            entity.HasIndex(x => x.Email)
                .IsUnique();

            entity.Property(x => x.PasswordHash)
                .HasMaxLength(500)
                .IsRequired();

            entity.Property(x => x.IsActive)
                .HasDefaultValue(true);

            entity.Property(x => x.CreatedAtUtc)
                .IsRequired();

            entity.Property(x => x.UpdatedAtUtc)
                .IsRequired();

            entity.HasOne(x => x.Role)
                  .WithMany(r => r.Users)
                  .HasForeignKey(x => x.RoleId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<OtpCode>(entity =>
        {
            entity.ToTable("OtpCodes");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.UserId).IsRequired();
            entity.Property(x => x.Code).HasMaxLength(10).IsRequired();
            entity.Property(x => x.SessionToken).HasMaxLength(50).IsRequired();
            entity.Property(x => x.ExpiresAtUtc).IsRequired();
            entity.Property(x => x.IsUsed).HasDefaultValue(false).IsRequired();
            entity.Property(x => x.CreatedAtUtc).IsRequired();

            entity.HasOne(x => x.User)
                  .WithMany()
                  .HasForeignKey(x => x.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}