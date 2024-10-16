namespace Rise.Shared.Users;

public interface IUserService
{
    Task<IEnumerable<UserDto.UserBase>> GetAllAsync();
    Task<UserDto.UserBase?> GetUserAsync();

    Task<UserDto.UserBase?> GetUserByIdAsync(int id);

    Task<UserDto.UserDetails?> GetUserDetailsByIdAsync(int id);

    Task<bool> CreateUserAsync(UserDto.RegistrationUser userDetails);

    Task<bool> UpdateUserAsync(UserDto.UpdateUser userDetails);

    Task<bool> DeleteUserAsync(int id);
    
    Task<IEnumerable<UserDto.UserTable>> GetUsersTableAsync();

}