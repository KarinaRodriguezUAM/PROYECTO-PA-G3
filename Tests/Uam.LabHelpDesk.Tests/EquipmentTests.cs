using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Moq;
using Uam.LabHelpDesk.Api.Data;
using Uam.LabHelpDesk.Api.DTOs;
using Uam.LabHelpDesk.Api.Models;
using Uam.LabHelpDesk.Api.Repositories;
using Xunit;

namespace Uam.LabHelpDesk.Tests;

public class EquipmentTests
{
    private (AppDbContext Context, EquipmentRepository Repository) CreateRepository()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new AppDbContext(options);

        var mockLocalizer = new Mock<IStringLocalizer<EquipmentRepository>>();
        mockLocalizer.Setup(l => l[It.IsAny<string>()])
            .Returns((string name) => new LocalizedString(name, name));

        var repository = new EquipmentRepository(context, mockLocalizer.Object);
        return (context, repository);
    }

    [Fact]
    public async Task CreateEquipment_ShouldFail_WhenCodeAlreadyExists()
    {
        // Arrange
        var (context, repository) = CreateRepository();

        var lab = new Laboratory
        {
            Id = 1,
            Name = "Lab 101",
            Building = "Bloque A",
            Floor = 1,
            Capacity = 30,
            IsActive = true
        };

        var existingEquipment = new Equipment
        {
            Id = 10,
            Code = "EQ-100",
            Brand = "HP",
            Model = "ProDesk",
            SerialNumber = "SN-100",
            Type = "PC",
            Status = "Operational",
            LaboratoryId = 1,
            IsActive = true
        };

        context.Laboratories.Add(lab);
        context.Equipment.Add(existingEquipment);
        await context.SaveChangesAsync();

        var createDto = new CreateEquipmentDto
        {
            Code = "eq-100", // Case insensitive duplicate check
            Brand = "Dell",
            Model = "OptiPlex",
            SerialNumber = "SN-200",
            Type = "PC",
            Status = "Operational",
            LaboratoryId = 1
        };

        // Act
        var result = await repository.CreateEquipmentAsync(createDto);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("EquipmentCodeExists", result.Message);
    }

    [Fact]
    public async Task CreateEquipment_ShouldFail_WhenLaboratoryDoesNotExist()
    {
        // Arrange
        var (context, repository) = CreateRepository();

        var createDto = new CreateEquipmentDto
        {
            Code = "EQ-999",
            Brand = "Lenovo",
            Model = "ThinkCentre",
            SerialNumber = "SN-999",
            Type = "PC",
            Status = "Operational",
            LaboratoryId = 999 // Non-existent Lab
        };

        // Act
        var result = await repository.CreateEquipmentAsync(createDto);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("LaboratoryNotFound", result.Message);
    }

    [Fact]
    public async Task DeleteEquipment_ShouldPerformLogicalDelete()
    {
        // Arrange
        var (context, repository) = CreateRepository();

        var equipment = new Equipment
        {
            Id = 25,
            Code = "EQ-025",
            Brand = "Epson",
            Model = "PowerLite",
            SerialNumber = "SN-025",
            Type = "Projector",
            Status = "Operational",
            LaboratoryId = 1,
            IsActive = true
        };

        context.Equipment.Add(equipment);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.DeleteEquipmentAsync(25);

        // Assert
        Assert.True(result.Success);

        var deletedItem = await context.Equipment.FindAsync(25);
        Assert.NotNull(deletedItem);
        Assert.False(deletedItem.IsActive);
    }
}
