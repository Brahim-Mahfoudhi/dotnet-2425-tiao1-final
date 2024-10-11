namespace Rise.Shared.Users;

public interface IUserService
{
    
    Task<List<UserDto>> GetAllAsync();
    Task<UserDto?> GetUserAsync();

    Task<UserDto?> GetUserByIdAsync(int id);

    Task<UserDetailsDto?> GetUserDetailsAsync(int id);

    Task<UserDetailsDto?> CreateUserAsync(UserDetailsDto userDetails);

    Task<bool> UpdateUserAsync(UserDetailsDto userDetails);

    Task<bool> DeleteUserAsync(int id);
}