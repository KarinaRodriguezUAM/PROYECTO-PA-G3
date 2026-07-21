using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Moq;
using Uam.LabHelpDesk.Api.Data;
using Uam.LabHelpDesk.Api.Models;
using Uam.LabHelpDesk.Api.Repositories;
using Xunit;

namespace Uam.LabHelpDesk.Tests;

public class DashboardTests
{
    private (AppDbContext Context, DashboardRepository Repository) CreateRepository()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new AppDbContext(options);

        var mockLocalizer = new Mock<IStringLocalizer<DashboardRepository>>();
        mockLocalizer.Setup(l => l[It.IsAny<string>()])
            .Returns((string name) => new LocalizedString(name, name));

        var repository = new DashboardRepository(context, mockLocalizer.Object);
        return (context, repository);
    }

    [Fact]
    public async Task GetGeneralSummary_ShouldReturnCorrectCounts()
    {
        // Arrange
        var (context, repository) = CreateRepository();

        var lab = new Laboratory { Id = 1, Name = "Lab 1", Building = "A", Floor = 1, Capacity = 20, IsActive = true };
        
        var eq1 = new Equipment { Id = 1, Code = "EQ-1", Brand = "Dell", Model = "M1", SerialNumber = "S1", Type = "PC", Status = "Operational", LaboratoryId = 1, IsActive = true };
        var eq2 = new Equipment { Id = 2, Code = "EQ-2", Brand = "Dell", Model = "M2", SerialNumber = "S2", Type = "PC", Status = "UnderRepair", LaboratoryId = 1, IsActive = true };

        var r1 = new FaultReport { Id = 1, EquipmentId = 1, ReportedByUserId = 1, Title = "Avería 1", Description = "Desc", Priority = "Low", Status = "Pending", ReportedAtUtc = DateTime.UtcNow, CreatedAtUtc = DateTime.UtcNow, UpdatedAtUtc = DateTime.UtcNow };
        var r2 = new FaultReport { Id = 2, EquipmentId = 1, ReportedByUserId = 1, Title = "Avería 2", Description = "Desc", Priority = "Medium", Status = "InProgress", ReportedAtUtc = DateTime.UtcNow, CreatedAtUtc = DateTime.UtcNow, UpdatedAtUtc = DateTime.UtcNow };
        var r3 = new FaultReport { Id = 3, EquipmentId = 2, ReportedByUserId = 1, Title = "Avería 3", Description = "Desc", Priority = "High", Status = "Resolved", ReportedAtUtc = DateTime.UtcNow, CreatedAtUtc = DateTime.UtcNow, UpdatedAtUtc = DateTime.UtcNow };
        var r4 = new FaultReport { Id = 4, EquipmentId = 2, ReportedByUserId = 1, Title = "Avería 4", Description = "Desc", Priority = "High", Status = "Closed", ReportedAtUtc = DateTime.UtcNow, CreatedAtUtc = DateTime.UtcNow, UpdatedAtUtc = DateTime.UtcNow };
        var r5 = new FaultReport { Id = 5, EquipmentId = 2, ReportedByUserId = 1, Title = "Avería 5", Description = "Desc", Priority = "Critical", Status = "Closed", ReportedAtUtc = DateTime.UtcNow, CreatedAtUtc = DateTime.UtcNow, UpdatedAtUtc = DateTime.UtcNow };

        context.Laboratories.Add(lab);
        context.Equipment.AddRange(eq1, eq2);
        context.FaultReports.AddRange(r1, r2, r3, r4, r5);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetGeneralSummaryAsync();

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Result);
        Assert.Equal(5, result.Result.TotalReports);
        Assert.Equal(1, result.Result.PendingCount);
        Assert.Equal(1, result.Result.InProgressCount);
        Assert.Equal(1, result.Result.ResolvedCount);
        Assert.Equal(2, result.Result.ClosedCount);
        Assert.Equal(2, result.Result.TotalEquipment);
        Assert.Equal(1, result.Result.EquipmentUnderRepair);
    }

    [Fact]
    public async Task GetReportsByLab_ShouldGroupCorrectlyByLaboratory()
    {
        // Arrange
        var (context, repository) = CreateRepository();

        var labA = new Laboratory { Id = 1, Name = "Lab Redes", Building = "A", Floor = 1, Capacity = 20, IsActive = true };
        var labB = new Laboratory { Id = 2, Name = "Lab Software", Building = "B", Floor = 2, Capacity = 30, IsActive = true };

        var eqA = new Equipment { Id = 1, Code = "EQ-A", Brand = "HP", Model = "P1", SerialNumber = "SA", Type = "PC", Status = "Operational", LaboratoryId = 1, IsActive = true };
        var eqB = new Equipment { Id = 2, Code = "EQ-B", Brand = "Lenovo", Model = "L1", SerialNumber = "SB", Type = "PC", Status = "Operational", LaboratoryId = 2, IsActive = true };

        var reportA = new FaultReport { Id = 10, EquipmentId = 1, ReportedByUserId = 1, Title = "Fallo A", Description = "Desc", Priority = "Low", Status = "Pending", ReportedAtUtc = DateTime.UtcNow, CreatedAtUtc = DateTime.UtcNow, UpdatedAtUtc = DateTime.UtcNow };
        var reportB = new FaultReport { Id = 20, EquipmentId = 2, ReportedByUserId = 1, Title = "Fallo B", Description = "Desc", Priority = "High", Status = "InProgress", ReportedAtUtc = DateTime.UtcNow, CreatedAtUtc = DateTime.UtcNow, UpdatedAtUtc = DateTime.UtcNow };

        context.Laboratories.AddRange(labA, labB);
        context.Equipment.AddRange(eqA, eqB);
        context.FaultReports.AddRange(reportA, reportB);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetReportsByLabAsync();

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Result);
        Assert.Equal(2, result.Result.Count);

        var labAResult = result.Result.FirstOrDefault(l => l.LabId == 1);
        Assert.NotNull(labAResult);
        Assert.Equal(1, labAResult.TotalReports);
        Assert.Equal(1, labAResult.PendingCount);
        Assert.Equal(0, labAResult.InProgressCount);

        var labBResult = result.Result.FirstOrDefault(l => l.LabId == 2);
        Assert.NotNull(labBResult);
        Assert.Equal(1, labBResult.TotalReports);
        Assert.Equal(0, labBResult.PendingCount);
        Assert.Equal(1, labBResult.InProgressCount);
    }

    [Fact]
    public async Task GetReportsByTechnician_ShouldReturnAssignedAndResolvedCounts()
    {
        // Arrange
        var (context, repository) = CreateRepository();

        var techRole = new Role { Id = 2, Name = "Technician", Description = "Técnico", IsActive = true };
        var technician = new User { Id = 5, FirstName = "Carlos", LastName = "Vargas", Email = "carlos@uam.edu", PasswordHash = "hash", RoleId = 2, Role = techRole, IsActive = true };

        var report1 = new FaultReport { Id = 1, EquipmentId = 1, ReportedByUserId = 1, AssignedToUserId = 5, Title = "Avería 1", Description = "Desc", Priority = "High", Status = "InProgress", ReportedAtUtc = DateTime.UtcNow, CreatedAtUtc = DateTime.UtcNow, UpdatedAtUtc = DateTime.UtcNow };

        var logResolved = new FaultReportStatusLog
        {
            Id = 1,
            FaultReportId = 1,
            ChangedByUserId = 5,
            PreviousStatus = "InProgress",
            NewStatus = "Resolved",
            ChangedAtUtc = DateTime.UtcNow
        };

        context.Roles.Add(techRole);
        context.Users.Add(technician);
        context.FaultReports.Add(report1);
        context.FaultReportStatusLogs.Add(logResolved);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetReportsByTechnicianAsync();

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Result);
        Assert.Single(result.Result);

        var techDto = result.Result.First();
        Assert.Equal(5, techDto.TechnicianId);
        Assert.Equal("Carlos Vargas", techDto.FullName);
        Assert.Equal(1, techDto.AssignedCount);
        Assert.Equal(1, techDto.ResolvedCount);
    }

    [Fact]
    public async Task GetAverageResolutionTime_ShouldCalculateCorrectDurationInHours()
    {
        // Arrange
        var (context, repository) = CreateRepository();

        var reportedDate = DateTime.UtcNow.AddHours(-10);
        var resolvedDate = DateTime.UtcNow.AddHours(-2);

        var report = new FaultReport
        {
            Id = 1,
            EquipmentId = 1,
            ReportedByUserId = 1,
            Title = "Avería resuelta",
            Description = "Desc",
            Priority = "High",
            Status = "Resolved",
            ReportedAtUtc = reportedDate,
            CreatedAtUtc = reportedDate,
            UpdatedAtUtc = resolvedDate
        };

        var log = new FaultReportStatusLog
        {
            Id = 1,
            FaultReportId = 1,
            ChangedByUserId = 5,
            PreviousStatus = "InProgress",
            NewStatus = "Resolved",
            ChangedAtUtc = resolvedDate
        };

        context.FaultReports.Add(report);
        context.FaultReportStatusLogs.Add(log);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetAverageResolutionTimeAsync();

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Result);
        Assert.Equal(8.0, result.Result.AverageHours);
        Assert.Equal(8.0, result.Result.FastestResolutionHours);
        Assert.Equal(8.0, result.Result.SlowestResolutionHours);
    }

    [Fact]
    public async Task GetReportsByStatus_ShouldGroupCountsByStatus()
    {
        // Arrange
        var (context, repository) = CreateRepository();

        var r1 = new FaultReport { Id = 1, EquipmentId = 1, ReportedByUserId = 1, Title = "A1", Description = "D", Priority = "Low", Status = "Pending", ReportedAtUtc = DateTime.UtcNow, CreatedAtUtc = DateTime.UtcNow, UpdatedAtUtc = DateTime.UtcNow };
        var r2 = new FaultReport { Id = 2, EquipmentId = 1, ReportedByUserId = 1, Title = "A2", Description = "D", Priority = "High", Status = "Pending", ReportedAtUtc = DateTime.UtcNow, CreatedAtUtc = DateTime.UtcNow, UpdatedAtUtc = DateTime.UtcNow };
        var r3 = new FaultReport { Id = 3, EquipmentId = 1, ReportedByUserId = 1, Title = "A3", Description = "D", Priority = "Medium", Status = "Resolved", ReportedAtUtc = DateTime.UtcNow, CreatedAtUtc = DateTime.UtcNow, UpdatedAtUtc = DateTime.UtcNow };

        context.FaultReports.AddRange(r1, r2, r3);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetReportsByStatusAsync();

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Result);

        var pendingItem = result.Result.FirstOrDefault(s => s.Status == "Pending");
        Assert.NotNull(pendingItem);
        Assert.Equal(2, pendingItem.Count);

        var resolvedItem = result.Result.FirstOrDefault(s => s.Status == "Resolved");
        Assert.NotNull(resolvedItem);
        Assert.Equal(1, resolvedItem.Count);
    }
}
