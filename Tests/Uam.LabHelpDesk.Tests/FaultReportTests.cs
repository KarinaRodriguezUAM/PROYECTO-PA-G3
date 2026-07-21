using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Moq;
using Uam.LabHelpDesk.Api.Data;
using Uam.LabHelpDesk.Api.DTOs;
using Uam.LabHelpDesk.Api.Models;
using Uam.LabHelpDesk.Api.Repositories;
using Xunit;

namespace Uam.LabHelpDesk.Tests;

public class FaultReportTests
{
    private (AppDbContext Context, FaultReportRepository Repository) CreateRepository()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new AppDbContext(options);

        var mockLocalizer = new Mock<IStringLocalizer<FaultReportRepository>>();
        mockLocalizer.Setup(l => l[It.IsAny<string>()])
            .Returns((string name) => new LocalizedString(name, name));

        var repository = new FaultReportRepository(context, mockLocalizer.Object);
        return (context, repository);
    }

    private async Task SeedBaseDataAsync(AppDbContext context)
    {
        var lab = new Laboratory
        {
            Id = 1,
            Name = "Lab Redes",
            Building = "Edificio B",
            Floor = 2,
            Capacity = 25,
            IsActive = true
        };

        var userReporter = new User
        {
            Id = 1,
            FirstName = "Instructor",
            LastName = "UAM",
            Email = "instructor@uam.edu",
            PasswordHash = "hash",
            RoleId = 3,
            IsActive = true
        };

        var userTech = new User
        {
            Id = 2,
            FirstName = "Tecnico",
            LastName = "UAM",
            Email = "tecnico@uam.edu",
            PasswordHash = "hash",
            RoleId = 2,
            IsActive = true
        };

        context.Laboratories.Add(lab);
        context.Users.AddRange(userReporter, userTech);
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task CreateFaultReport_ShouldFail_WhenEquipmentIsUnderRepair()
    {
        // Arrange
        var (context, repository) = CreateRepository();
        await SeedBaseDataAsync(context);

        var equipment = new Equipment
        {
            Id = 1,
            Code = "EQ-001",
            Brand = "Dell",
            Model = "OptiPlex",
            SerialNumber = "SN-001",
            Type = "PC",
            Status = "UnderRepair",
            LaboratoryId = 1,
            IsActive = true
        };

        context.Equipment.Add(equipment);
        await context.SaveChangesAsync();

        var createDto = new CreateFaultReportDto
        {
            EquipmentId = 1,
            Title = "Pantalla dañada",
            Description = "No enciende el monitor",
            Priority = "High"
        };

        // Act
        var result = await repository.CreateFaultReportAsync(createDto, reportedByUserId: 1);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("EquipmentNotOperational", result.Message);
    }

    [Fact]
    public async Task UpdateFaultReportStatus_ShouldFail_WhenReportIsClosed()
    {
        // Arrange
        var (context, repository) = CreateRepository();
        await SeedBaseDataAsync(context);

        var equipment = new Equipment
        {
            Id = 1,
            Code = "EQ-001",
            Brand = "Dell",
            Model = "OptiPlex",
            SerialNumber = "SN-001",
            Type = "PC",
            Status = "UnderRepair",
            LaboratoryId = 1,
            IsActive = true
        };

        var report = new FaultReport
        {
            Id = 100,
            EquipmentId = 1,
            ReportedByUserId = 1,
            AssignedToUserId = 2,
            Title = "Error de red",
            Description = "Sin conexión a internet",
            Priority = "Medium",
            Status = "Closed",
            ReportedAtUtc = DateTime.UtcNow,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };

        context.Equipment.Add(equipment);
        context.FaultReports.Add(report);
        await context.SaveChangesAsync();

        var updateDto = new UpdateFaultReportStatusDto
        {
            Notes = "Intentando reabrir"
        };

        // Act
        var result = await repository.UpdateFaultReportStatusAsync(report.Id, updateDto, changedByUserId: 2);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("FaultReportClosed", result.Message);
    }

    [Fact]
    public async Task UpdateFaultReportStatus_ShouldFail_WhenTransitionIsInvalid()
    {
        // Arrange
        var (context, repository) = CreateRepository();
        await SeedBaseDataAsync(context);

        var equipment = new Equipment
        {
            Id = 1,
            Code = "EQ-001",
            Brand = "Dell",
            Model = "OptiPlex",
            SerialNumber = "SN-001",
            Type = "PC",
            Status = "UnderRepair",
            LaboratoryId = 1,
            IsActive = true
        };

        var report = new FaultReport
        {
            Id = 101,
            EquipmentId = 1,
            ReportedByUserId = 1,
            AssignedToUserId = 2,
            Title = "Fallo de software",
            Description = "Programa no abre",
            Priority = "Low",
            Status = "Pending", // Direct pending -> resolved transition is invalid without in-progress
            ReportedAtUtc = DateTime.UtcNow,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };

        context.Equipment.Add(equipment);
        context.FaultReports.Add(report);
        await context.SaveChangesAsync();

        var updateDto = new UpdateFaultReportStatusDto
        {
            Notes = "Intento de resolver directamente"
        };

        // Act
        var result = await repository.UpdateFaultReportStatusAsync(report.Id, updateDto, changedByUserId: 2);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("InvalidTransition", result.Message);
    }

    [Fact]
    public async Task AssignFaultReport_ShouldSucceed_WhenReportIsPending()
    {
        // Arrange
        var (context, repository) = CreateRepository();
        await SeedBaseDataAsync(context);

        var equipment = new Equipment
        {
            Id = 1,
            Code = "EQ-001",
            Brand = "Dell",
            Model = "OptiPlex",
            SerialNumber = "SN-001",
            Type = "PC",
            Status = "UnderRepair",
            LaboratoryId = 1,
            IsActive = true
        };

        var report = new FaultReport
        {
            Id = 102,
            EquipmentId = 1,
            ReportedByUserId = 1,
            Title = "Teclado descompuesto",
            Description = "Teclas pegadas",
            Priority = "Low",
            Status = "Pending",
            ReportedAtUtc = DateTime.UtcNow,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };

        context.Equipment.Add(equipment);
        context.FaultReports.Add(report);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.AssignFaultReportAsync(report.Id, technicianUserId: 2);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("InProgress", result.Result!.Status);
        Assert.Equal(2, result.Result.AssignedToUserId);

        var log = await context.FaultReportStatusLogs.FirstOrDefaultAsync(l => l.FaultReportId == report.Id);
        Assert.NotNull(log);
        Assert.Equal("Pending", log.PreviousStatus);
        Assert.Equal("InProgress", log.NewStatus);
    }

    [Fact]
    public async Task CloseFaultReport_ShouldFail_WhenReportIsNotInResolvedState()
    {
        // Arrange
        var (context, repository) = CreateRepository();
        await SeedBaseDataAsync(context);

        var equipment = new Equipment
        {
            Id = 1,
            Code = "EQ-001",
            Brand = "Dell",
            Model = "OptiPlex",
            SerialNumber = "SN-001",
            Type = "PC",
            Status = "UnderRepair",
            LaboratoryId = 1,
            IsActive = true
        };

        var report = new FaultReport
        {
            Id = 103,
            EquipmentId = 1,
            ReportedByUserId = 1,
            Title = "Virus en sistema",
            Description = "Infección detectada",
            Priority = "Critical",
            Status = "InProgress", // Cannot close unless Resolved
            ReportedAtUtc = DateTime.UtcNow,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };

        context.Equipment.Add(equipment);
        context.FaultReports.Add(report);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.CloseFaultReportAsync(report.Id);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("FaultReportMustBeResolved", result.Message);
    }
}
