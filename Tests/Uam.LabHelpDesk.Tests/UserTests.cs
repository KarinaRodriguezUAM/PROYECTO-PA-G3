using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Moq;
using Uam.LabHelpDesk.Api.Data;
using Uam.LabHelpDesk.Api.DTOs;
using Uam.LabHelpDesk.Api.Models;
using Uam.LabHelpDesk.Api.Repositories;
using Xunit;

namespace Uam.LabHelpDesk.Tests;

public class UserTests
{
    private (AppDbContext Context, UserRepository Repository) CreateRepository()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new AppDbContext(options);

        var mockLocalizer = new Mock<IStringLocalizer<UserRepository>>();
        mockLocalizer.Setup(l => l[It.IsAny<string>()])
            .Returns((string name) => new LocalizedString(name, name));

        var repository = new UserRepository(context, mockLocalizer.Object);
        return (context, repository);
    }

    [Fact]
    public async Task CreateUser_ShouldFail_WhenEmailAlreadyExists()
    {
        // Arrange
        var (context, repository) = CreateRepository();

        var existingUser = new User
        {
            Id = 1,
            FirstName = "Juan",
            LastName = "Pérez",
            Email = "duplicado@uam.edu",
            PasswordHash = "hash123",
            RoleId = 1,
            IsActive = true
        };

        context.Users.Add(existingUser);
        await context.SaveChangesAsync();

        var createUserDto = new CreateUserDto
        {
            FirstName = "Pedro",
            LastName = "Gómez",
            Email = "DUPLICADO@uam.edu", // Case insensitive check
            Password = "Password123!",
            RoleId = 1
        };

        // Act
        var result = await repository.CreateUserAsync(createUserDto);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("EmailAlreadyExists", result.Message);
    }

    [Fact]
    public async Task CreateUser_ShouldFail_WhenRoleIdIsInactiveOrNotFound()
    {
        // Arrange
        var (context, repository) = CreateRepository();

        var inactiveRole = new Role
        {
            Id = 10,
            Name = "InactivoRole",
            Description = "Rol inactivo",
            IsActive = false
        };

        context.Roles.Add(inactiveRole);
        await context.SaveChangesAsync();

        var createUserDto = new CreateUserDto
        {
            FirstName = "Maria",
            LastName = "López",
            Email = "maria.lopez@uam.edu",
            Password = "Password123!",
            RoleId = 10 // Inactive role
        };

        // Act
        var result = await repository.CreateUserAsync(createUserDto);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("RoleNotActive", result.Message);
    }

    [Fact]
    public async Task GetUserById_ShouldReturnUserDto_WhenUserExists()
    {
        // Arrange
        var (context, repository) = CreateRepository();

        var role = new Role
        {
            Id = 3,
            Name = "Instructor",
            Description = "Docente",
            IsActive = true
        };

        var user = new User
        {
            Id = 50,
            FirstName = "Ana",
            LastName = "Sánchez",
            Email = "ana.sanchez@uam.edu",
            PasswordHash = "hashedPassword",
            RoleId = 3,
            Role = role,
            IsActive = true
        };

        context.Roles.Add(role);
        context.Users.Add(user);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetUserByIdAsync(50);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("ana.sanchez@uam.edu", result.Result!.Email);
        Assert.Equal("Instructor", result.Result.RoleName);
    }
}
