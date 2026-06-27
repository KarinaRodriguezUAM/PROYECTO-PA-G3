using Microsoft.EntityFrameworkCore;
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
    IStringLocalizer<UserRepository> userLocalizer) : IUnitOfWork
{
    private ILaboratoryRepository? _laboratories;
    private IEquipmentRepository? _equipment;
    private IRoleRepository? _roles;
    private IUserRepository? _users;
    private IRefreshTokenRepository? _refreshTokens;
    private IOtpCodeRepository? _otpCodes;

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

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken);
}