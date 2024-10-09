namespace Rise.Shared.Users;

public interface IUserService
{
    Task<UserDto?> GetUserAsync();

    Task<UserDto?> GetUserByIdAsync(int id);

    Task<UserDto?> GetUserDetailsAsync(int id);

    Task<UserDto?> CreateUserAsync(UserDto user);

    Task<bool> UpdateUserAsync(UserDto user);

    Task<bool> DeleteUserAsync(int id);
}