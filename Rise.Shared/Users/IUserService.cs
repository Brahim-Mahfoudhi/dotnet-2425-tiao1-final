namespace Rise.Shared.Users;

public interface IUserService
{
    Task<List<UserDto.GetUser>> GetAllAsync();
    Task<UserDto.GetUser?> GetUserAsync();

    Task<UserDto.GetUser?> GetUserByIdAsync(int id);

    Task<UserDto.GetUserDetails?> GetUserDetailsByIdAsync(int id);

    Task<bool> CreateUserAsync(UserDto.CreateUser userDetails);

    Task<bool> UpdateUserAsync(UserDto.UpdateUser userDetails);

    Task<bool> DeleteUserAsync(int id);
}
