using Uam.LabHelpDesk.Api.DTOs;
using Uam.LabHelpDesk.Api.Models;

namespace Uam.LabHelpDesk.Api.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<bool> EmailExistsAsync(
        string email,
        int? excludeId = null,
        CancellationToken cancellationToken = default);

    Task<ApiOperationResultDto<List<UserDto>>> GetAllUsersAsync(
        CancellationToken cancellationToken = default);

    Task<ApiOperationResultDto<UserDto>> GetUserByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<ApiOperationResultDto<List<UserDto>>> GetUsersByRoleAsync(
        int roleId,
        CancellationToken cancellationToken = default);

    Task<ApiOperationResultDto<UserDto>> CreateUserAsync(
        CreateUserDto resource,
        CancellationToken cancellationToken = default);

    Task<ApiOperationResultDto<UserDto>> UpdateUserAsync(
        int id,
        UpdateUserDto resource,
        CancellationToken cancellationToken = default);

    Task<ApiOperationResultDto<object>> DeleteUserAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
}