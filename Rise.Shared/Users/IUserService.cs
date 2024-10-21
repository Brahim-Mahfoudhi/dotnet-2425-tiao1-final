namespace Rise.Shared.Users;

public interface IUserService
{
    Task<IEnumerable<UserDto.UserBase>?> GetAllAsync();
    Task<UserDto.UserBase?> GetUserAsync();

    Task<UserDto.UserBase?> GetUserByIdAsync(string id);

    Task<UserDto.UserDetails?> GetUserDetailsByIdAsync(string id);

    Task<bool> CreateUserAsync(UserDto.RegistrationUser userDetails);

    Task<bool> UpdateUserAsync(UserDto.UpdateUser userDetails);

    Task<bool> DeleteUserAsync(string id);

    Task<IEnumerable<UserDto.Auth0User>> GetAuth0Users();

}