using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Moq;
using Uam.LabHelpDesk.Api.Data;
using Uam.LabHelpDesk.Api.DTOs;
using Uam.LabHelpDesk.Api.Models;
using Uam.LabHelpDesk.Api.Repositories;
using Xunit;

namespace Uam.LabHelpDesk.Tests;

public class RoleTests
{
    private (AppDbContext Context, RoleRepository Repository) CreateRepository()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new AppDbContext(options);

        var mockLocalizer = new Mock<IStringLocalizer<RoleRepository>>();
        mockLocalizer.Setup(l => l[It.IsAny<string>()])
            .Returns((string name) => new LocalizedString(name, name));

        var repository = new RoleRepository(context, mockLocalizer.Object);
        return (context, repository);
    }

    [Fact]
    public async Task CreateRole_ShouldFail_WhenRoleNameAlreadyExists()
    {
        // Arrange
        var (context, repository) = CreateRepository();

        var existingRole = new Role
        {
            Id = 1,
            Name = "Administrator",
            Description = "Administrador del sistema",
            IsActive = true
        };

        context.Roles.Add(existingRole);
        await context.SaveChangesAsync();

        var createDto = new CreateRoleDto
        {
            Name = "administrator", // Case insensitive check
            Description = "Duplicado de administrador"
        };

        // Act
        var result = await repository.CreateRoleAsync(createDto);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("RoleNameExists", result.Message);
    }

    [Fact]
    public async Task DeleteRole_ShouldPerformLogicalDelete_SettingIsActiveFalse()
    {
        // Arrange
        var (context, repository) = CreateRepository();

        var role = new Role
        {
            Id = 4,
            Name = "Auditor",
            Description = "Rol de auditoría",
            IsActive = true
        };

        context.Roles.Add(role);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.DeleteRoleAsync(4);

        // Assert
        Assert.True(result.Success);

        var deletedRole = await context.Roles.FindAsync(4);
        Assert.NotNull(deletedRole);
        Assert.False(deletedRole.IsActive);
    }
}
