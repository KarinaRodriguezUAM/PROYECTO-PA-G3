using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Moq;
using Uam.LabHelpDesk.Api.DTOs.Auth;
using Uam.LabHelpDesk.Api.Interfaces;
using Uam.LabHelpDesk.Api.Models;
using Uam.LabHelpDesk.Api.Repositories;
using Uam.LabHelpDesk.Api.Services;
using Xunit;

namespace Uam.LabHelpDesk.Tests;

public class AuthAndNotificationTests
{
    [Fact]
    public async Task LoginAsync_ShouldInvokeSmtpService_WhenCredentialsAreValid()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockUserRepo = new Mock<IUserRepository>();
        var mockOtpRepo = new Mock<IOtpCodeRepository>();
        var mockSmtpService = new Mock<ISmtpService>();
        var mockLocalizer = new Mock<IStringLocalizer<AuthRepository>>();

        mockLocalizer.Setup(l => l[It.IsAny<string>()])
            .Returns((string name) => new LocalizedString(name, name));

        var validUser = new User
        {
            Id = 1,
            Email = "usuario.valido@uam.edu",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
            IsActive = true,
            RoleId = 1
        };

        mockUserRepo.Setup(r => r.GetByEmailAsync("usuario.valido@uam.edu", It.IsAny<CancellationToken>()))
            .ReturnsAsync(validUser);

        mockOtpRepo.Setup(r => r.GetActiveOtpCodesByUserIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<OtpCode>());

        mockUnitOfWork.Setup(u => u.Users).Returns(mockUserRepo.Object);
        mockUnitOfWork.Setup(u => u.OtpCodes).Returns(mockOtpRepo.Object);

        mockSmtpService
            .Setup(s => s.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(true);

        var inMemoryConfig = new Dictionary<string, string?>
        {
            { "OtpExpirationMinutes", "10" }
        };
        var config = new ConfigurationBuilder().AddInMemoryCollection(inMemoryConfig).Build();

        var authRepository = new AuthRepository(
            mockUnitOfWork.Object,
            config,
            mockLocalizer.Object,
            mockSmtpService.Object);

        var loginRequest = new LoginRequestDto
        {
            Email = "usuario.valido@uam.edu",
            Password = "Password123!"
        };

        // Act
        var result = await authRepository.LoginAsync(loginRequest);

        // Assert
        Assert.True(result.Success);
        mockSmtpService.Verify(
            s => s.SendEmailAsync(
                "usuario.valido@uam.edu",
                It.Is<string>(subj => subj.Contains("verificación")),
                It.IsAny<string>()),
            Times.Once,
            "El servicio de notificaciones SMTP debe invocarse exactamente una vez al generar OTP sin enviar correos reales."
        );
    }

    [Fact]
    public async Task LoginAsync_ShouldNotInvokeSmtpService_WhenPasswordIsInvalid()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockUserRepo = new Mock<IUserRepository>();
        var mockSmtpService = new Mock<ISmtpService>();
        var mockLocalizer = new Mock<IStringLocalizer<AuthRepository>>();

        mockLocalizer.Setup(l => l[It.IsAny<string>()])
            .Returns((string name) => new LocalizedString(name, name));

        var validUser = new User
        {
            Id = 2,
            Email = "usuario@uam.edu",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("CorrectPassword123!"),
            IsActive = true
        };

        mockUserRepo.Setup(r => r.GetByEmailAsync("usuario@uam.edu", It.IsAny<CancellationToken>()))
            .ReturnsAsync(validUser);

        mockUnitOfWork.Setup(u => u.Users).Returns(mockUserRepo.Object);

        var config = new ConfigurationBuilder().Build();

        var authRepository = new AuthRepository(
            mockUnitOfWork.Object,
            config,
            mockLocalizer.Object,
            mockSmtpService.Object);

        var invalidLoginRequest = new LoginRequestDto
        {
            Email = "usuario@uam.edu",
            Password = "WrongPassword!" // Invalid password
        };

        // Act
        var result = await authRepository.LoginAsync(invalidLoginRequest);

        // Assert
        Assert.False(result.Success);
        mockSmtpService.Verify(
            s => s.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
            Times.Never,
            "El servicio de notificaciones SMTP NO debe invocarse cuando la contraseña es incorrecta."
        );
    }

    [Fact]
    public async Task SmtpService_SendEmailAsync_ShouldBypass_InDevelopmentEnvironment()
    {
        // Arrange
        var inMemorySettings = new Dictionary<string, string?>
        {
            { "Smtp:Host", "smtp.gmail.com" },
            { "Smtp:Port", "587" },
            { "Smtp:SenderEmail", "sender@gmail.com" },
            { "Smtp:SenderName", "UAM Lab Help Desk" },
            { "Smtp:Password", "pass" } // Development bypass password
        };

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var mockLogger = new Mock<ILogger<SmtpService>>();

        var smtpService = new SmtpService(config, mockLogger.Object);

        // Act
        var result = await smtpService.SendEmailAsync("test@uam.edu", "Asunto de prueba", "<p>Cuerpo</p>");

        // Assert
        Assert.True(result);
    }
}
