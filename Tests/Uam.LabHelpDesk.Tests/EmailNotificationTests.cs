using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Moq;
using Uam.LabHelpDesk.Api.Data;
using Uam.LabHelpDesk.Api.Interfaces;
using Uam.LabHelpDesk.Api.Models;
using Uam.LabHelpDesk.Api.Services;
using Xunit;

namespace Uam.LabHelpDesk.Tests;

public class EmailNotificationTests
{
    private (AppDbContext Context, EmailNotificationService Service, Mock<ISmtpService> MockSmtp) CreateService()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new AppDbContext(options);
        var mockSmtp = new Mock<ISmtpService>();
        mockSmtp.Setup(s => s.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(true);

        var mockLocalizer = new Mock<IStringLocalizer<EmailNotificationService>>();
        mockLocalizer.Setup(l => l[It.IsAny<string>()])
            .Returns((string name) => new LocalizedString(name, "Template {0} {1} {2} {3} {4}"));

        var mockLogger = new Mock<ILogger<EmailNotificationService>>();

        var service = new EmailNotificationService(context, mockSmtp.Object, mockLocalizer.Object, mockLogger.Object);
        return (context, service, mockSmtp);
    }

    [Fact]
    public async Task SendReportCreatedAsync_ShouldSendEmailToActiveTechniciansOnly()
    {
        // Arrange
        var (context, service, mockSmtp) = CreateService();

        var techRole = new Role { Id = 2, Name = "Technician", IsActive = true };
        var instructorRole = new Role { Id = 3, Name = "Instructor", IsActive = true };

        var activeTech = new User
        {
            Id = 1,
            FirstName = "Carlos",
            LastName = "Técnico",
            Email = "tech1.activo@uam.edu",
            RoleId = 2,
            Role = techRole,
            IsActive = true
        };

        var inactiveTech = new User
        {
            Id = 2,
            FirstName = "Ana",
            LastName = "Técnico Inactivo",
            Email = "tech2.inactivo@uam.edu",
            RoleId = 2,
            Role = techRole,
            IsActive = false
        };

        var instructor = new User
        {
            Id = 3,
            FirstName = "Diego",
            LastName = "Profesor",
            Email = "profe@uam.edu",
            RoleId = 3,
            Role = instructorRole,
            IsActive = true
        };

        var lab = new Laboratory { Id = 1, Name = "Lab Redes", Building = "A", Floor = 1, Capacity = 20, IsActive = true };
        var equipment = new Equipment { Id = 10, Code = "EQ-010", Brand = "Dell", Model = "OptiPlex", SerialNumber = "SN10", Type = "PC", Status = "UnderRepair", LaboratoryId = 1, Laboratory = lab, IsActive = true };

        context.Roles.AddRange(techRole, instructorRole);
        context.Users.AddRange(activeTech, inactiveTech, instructor);
        context.Laboratories.Add(lab);
        context.Equipment.Add(equipment);
        await context.SaveChangesAsync();

        var report = new FaultReport
        {
            Id = 100,
            Title = "Monitor sin imagen",
            Description = "Fallo de pantalla",
            Priority = "High",
            Status = "Pending",
            EquipmentId = 10,
            Equipment = equipment,
            ReportedByUserId = 3,
            ReportedByUser = instructor,
            ReportedAtUtc = DateTime.UtcNow
        };

        // Act
        await service.SendReportCreatedAsync(report);
        await Task.Delay(200); // Dar tiempo a que finalice Task.Run de fondo

        // Assert
        mockSmtp.Verify(
            s => s.SendEmailAsync("tech1.activo@uam.edu", It.IsAny<string>(), It.IsAny<string>()),
            Times.Once,
            "Debe enviarle correo al técnico activo.");

        mockSmtp.Verify(
            s => s.SendEmailAsync("tech2.inactivo@uam.edu", It.IsAny<string>(), It.IsAny<string>()),
            Times.Never,
            "NO debe enviarle correo al técnico inactivo.");
    }

    [Fact]
    public async Task SendReportAssignedAsync_ShouldNotSendEmail_WhenTechnicianIsInactive()
    {
        // Arrange
        var (context, service, mockSmtp) = CreateService();

        var inactiveTech = new User
        {
            Id = 5,
            FirstName = "Juan",
            LastName = "Inactivo",
            Email = "tech.inactivo@uam.edu",
            RoleId = 2,
            IsActive = false
        };

        var report = new FaultReport
        {
            Id = 101,
            Title = "Teclado roto",
            Status = "InProgress"
        };

        // Act
        await service.SendReportAssignedAsync(report, inactiveTech);
        await Task.Delay(200); // Dar tiempo a que finalice Task.Run de fondo

        // Assert
        mockSmtp.Verify(
            s => s.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
            Times.Never,
            "No debe enviar correo si el técnico asignado está inactivo.");
    }

    [Fact]
    public async Task SendStatusChangedAsync_ShouldSendEmailToInstructor_WhenReportIsResolved()
    {
        // Arrange
        var (context, service, mockSmtp) = CreateService();

        var instructor = new User
        {
            Id = 10,
            FirstName = "Maria",
            Email = "instructor.creador@uam.edu",
            IsActive = true
        };

        context.Users.Add(instructor);
        await context.SaveChangesAsync();

        var report = new FaultReport
        {
            Id = 102,
            Title = "Mouse defectuoso",
            Status = "Resolved",
            ReportedByUserId = 10,
            ReportedByUser = instructor
        };

        // Act
        await service.SendStatusChangedAsync(report, "Resolved");
        await Task.Delay(200); // Dar tiempo a que finalice Task.Run de fondo

        // Assert
        mockSmtp.Verify(
            s => s.SendEmailAsync("instructor.creador@uam.edu", It.IsAny<string>(), It.IsAny<string>()),
            Times.Once,
            "Debe notificar al instructor creador cuando la avería se resuelve.");
    }

    [Fact]
    public async Task SendReportClosedAsync_ShouldSendEmailToInstructor_WhenReportIsClosed()
    {
        // Arrange
        var (context, service, mockSmtp) = CreateService();

        var instructor = new User
        {
            Id = 11,
            FirstName = "Roberto",
            Email = "roberto.instructor@uam.edu",
            IsActive = true
        };

        context.Users.Add(instructor);
        await context.SaveChangesAsync();

        var report = new FaultReport
        {
            Id = 103,
            Title = "Proyector sin señal",
            Status = "Closed",
            ReportedByUserId = 11,
            ReportedByUser = instructor
        };

        // Act
        await service.SendReportClosedAsync(report);
        await Task.Delay(200); // Dar tiempo a que finalice Task.Run de fondo

        // Assert
        mockSmtp.Verify(
            s => s.SendEmailAsync("roberto.instructor@uam.edu", It.IsAny<string>(), It.IsAny<string>()),
            Times.Once,
            "Debe notificar al instructor creador cuando el reporte se cierra.");
    }
}
