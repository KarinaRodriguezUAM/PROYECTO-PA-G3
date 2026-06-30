namespace Uam.LabHelpDesk.Api.Interfaces
{
    public interface IUnitOfWork
    {
        ILaboratoryRepository Laboratories { get; }

        IEquipmentRepository Equipment { get; }

        IRoleRepository Roles { get; }

        IUserRepository Users { get; }

        IRefreshTokenRepository RefreshTokens { get; }

        IOtpCodeRepository OtpCodes { get; }

        IPasswordResetRepository PasswordResets { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}