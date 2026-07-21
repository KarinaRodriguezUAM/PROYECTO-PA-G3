using Microsoft.Extensions.Localization;
using Uam.LabHelpDesk.Api.Data;
using Uam.LabHelpDesk.Api.Interfaces;
using Uam.LabHelpDesk.Api.Models;

namespace Uam.LabHelpDesk.Api.Repositories;

public class UnitOfWork(
    AppDbContext context,
    IStringLocalizer<LaboratoryRepository> laboratoryLocalizer,
    IStringLocalizer<EquipmentRepository> equipmentLocalizer,
    IStringLocalizer<RoleRepository> roleLocalizer,
    IStringLocalizer<UserRepository> userLocalizer,
    IStringLocalizer<FaultReportRepository> faultReportLocalizer,
    IStringLocalizer<FaultReportStatusLogRepository> statusLogLocalizer,
    IStringLocalizer<DashboardRepository>? dashboardLocalizer = null,
    IEmailNotificationService? emailNotificationService = null,
    ILogger<FaultReportRepository>? faultReportLogger = null) : IUnitOfWork 
{

    private ILaboratoryRepository? _laboratories;
    private IEquipmentRepository? _equipment;
    private IRoleRepository? _roles;
    private IUserRepository? _users;
    private IRefreshTokenRepository? _refreshTokens;
    private IOtpCodeRepository? _otpCodes;
    private IFaultReportRepository? _faultReports;
    private IFaultReportStatusLogRepository? _statusLogs;
    private IDashboardRepository? _dashboard;

    public ILaboratoryRepository Laboratories =>
        _laboratories ??= new LaboratoryRepository(context, laboratoryLocalizer);

    public IEquipmentRepository Equipment =>
        _equipment ??= new EquipmentRepository(context, equipmentLocalizer);

    public IRoleRepository Roles =>
        _roles ??= new RoleRepository(context, roleLocalizer);

    public IUserRepository Users =>
        _users ??= new UserRepository(context, userLocalizer);

    public IRefreshTokenRepository RefreshTokens =>
        _refreshTokens ??= new RefreshTokenRepository(context);

    public IOtpCodeRepository OtpCodes =>
        _otpCodes ??= new OtpCodeRepository(context);

    public IFaultReportRepository FaultReports =>
        _faultReports ??= new FaultReportRepository(context, faultReportLocalizer, emailNotificationService, faultReportLogger);

    public IFaultReportStatusLogRepository StatusLogs =>
        _statusLogs ??= new FaultReportStatusLogRepository(context, statusLogLocalizer);

    public IDashboardRepository Dashboard =>
        _dashboard ??= new DashboardRepository(context, dashboardLocalizer ?? new Microsoft.Extensions.Localization.StringLocalizer<DashboardRepository>(new Microsoft.Extensions.Localization.ResourceManagerStringLocalizerFactory(Microsoft.Extensions.Options.Options.Create(new Microsoft.Extensions.Localization.LocalizationOptions { ResourcesPath = "Resources" }), new Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory())));

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken);
    public IPasswordResetRepository PasswordResets =>
    _passwordResets ??= new PasswordResetRepository(context);

    private IPasswordResetRepository? _passwordResets;
}