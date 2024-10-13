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

    public async Task<bool> CreateUserAsync(UserDto.CreateUser userDetails)
    {
        Console.WriteLine(JsonSerializer.Serialize(userDetails, jsonSerializerOptions));
        var response = await httpClient.PostAsJsonAsync("user", userDetails);
        Console.WriteLine($"response: {response}");
        return true;
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        var response = await httpClient.DeleteAsync($"user/{id}");
        return response.IsSuccessStatusCode;
    }

    public async Task<IEnumerable<UserDto.GetUser>?> GetAllAsync()
    {
        var jsonResponse = await httpClient.GetStringAsync("user/all");
        return JsonSerializer.Deserialize<IEnumerable<UserDto.GetUser>>(jsonResponse, jsonSerializerOptions); ;
    }

    public Task<UserDto.GetUser?> GetUserAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<UserDto.GetUser?> GetUserByIdAsync(int id)
    {
        var jsonResponse = await httpClient.GetStringAsync($"user/{id}");
        return JsonSerializer.Deserialize<UserDto.GetUser>(jsonResponse, jsonSerializerOptions);
    }

    public async Task<UserDto.GetUserDetails?> GetUserDetailsByIdAsync(int id)
    {
        var jsonResponse = await httpClient.GetStringAsync($"user/details/{id}");
        return JsonSerializer.Deserialize<UserDto.GetUserDetails>(jsonResponse, jsonSerializerOptions);
    }

    public async Task<bool> UpdateUserAsync(int id, UserDto.UpdateUser userDetails)
    {
        var response = await httpClient.PutAsJsonAsync($"user/{id}", userDetails);
        return response.IsSuccessStatusCode;
    }
}
