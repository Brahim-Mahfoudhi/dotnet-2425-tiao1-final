using Rise.Shared.Users;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Rise.Client.Users;

public class UserService : IUserService
{
    private readonly HttpClient httpClient;
    private readonly JsonSerializerOptions jsonSerializerOptions;

    public UserService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
        this.jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        this.jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    }

    public async Task<bool> CreateUserAsync(UserDto.RegistrationUser userDetails)
    {
        Console.WriteLine(JsonSerializer.Serialize(userDetails, jsonSerializerOptions));
        var response = await httpClient.PostAsJsonAsync("user/create", userDetails);
        Console.WriteLine($"response: {response}");
        return true;
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        var response = await httpClient.DeleteAsync($"user/{id}");
        return response.IsSuccessStatusCode;
    }

    public async Task<IEnumerable<UserDto.UserBase>?> GetAllAsync()
    {
        var jsonResponse = await httpClient.GetStringAsync("user/all");
        return JsonSerializer.Deserialize<IEnumerable<UserDto.UserBase>>(jsonResponse, jsonSerializerOptions); ;
    }

    public Task<UserDto.UserBase?> GetUserAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<UserDto.UserBase?> GetUserByIdAsync(int id)
    {
        var jsonResponse = await httpClient.GetStringAsync($"user/{id}");
        return JsonSerializer.Deserialize<UserDto.UserBase>(jsonResponse, jsonSerializerOptions);
    }

    public async Task<UserDto.UserDetails?> GetUserDetailsByIdAsync(int id)
    {
        var jsonResponse = await httpClient.GetStringAsync($"user/details/{id}");
        return JsonSerializer.Deserialize<UserDto.UserDetails>(jsonResponse, jsonSerializerOptions);
    }

    public async Task<bool> UpdateUserAsync(UserDto.UpdateUser userDetails)
    {
        var response = await httpClient.PutAsJsonAsync($"user/{userDetails.Id}", userDetails);
        return response.IsSuccessStatusCode;
    }
    
    public async Task<IEnumerable<UserDto.Auth0User>> GetAuth0Users()
    { 
        var users = await httpClient.GetFromJsonAsync<IEnumerable<UserDto.Auth0User>>("user/auth/users");
        return users!;
    }
}
