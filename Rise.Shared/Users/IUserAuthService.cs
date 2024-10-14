using System;
namespace Rise.Shared.Users;
public interface IUserAuthService
{
    Task<IEnumerable<UserAuthDto.Index>> GetUsersAsync();
}