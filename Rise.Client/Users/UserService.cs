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

    public async Task<(bool Success, string? Message)> CreateUserAsync(UserDto.RegistrationUser userDetails)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync("user", userDetails);
            if (response.IsSuccessStatusCode)
            {
                return (true, "User created successfully");
            }
            else
            {
                var errorContent = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                var message = errorContent != null && errorContent.TryGetValue("message", out var errorMessage)
                    ? errorMessage
                    : "Failed to create user due to an unknown error.";
                return (false, message);
            }
        }
        catch (Exception ex)
        {
            return (false, $"An unexpected error occurred: {ex.Message}");
        }
    }

    public async Task<bool> DeleteUserAsync(string userid)
    {
        var response = await httpClient.DeleteAsync($"user/{userid}");
        return response.IsSuccessStatusCode;
    }

    public async Task<IEnumerable<UserDto.UserBase>?> GetAllAsync()
    {
        var jsonResponse = await httpClient.GetStringAsync("user");
        return JsonSerializer.Deserialize<IEnumerable<UserDto.UserBase>>(jsonResponse, jsonSerializerOptions);
    }

    public async Task<UserDto.UserBase?> GetUserByIdAsync(string userid)
    {
        var jsonResponse = await httpClient.GetStringAsync($"user/{userid}");
        return JsonSerializer.Deserialize<UserDto.UserBase>(jsonResponse, jsonSerializerOptions);
    }

    public async Task<UserDto.UserDetails?> GetUserDetailsByIdAsync(string userid)
    {
        var jsonResponse = await httpClient.GetStringAsync($"user/{userid}/details");
        return JsonSerializer.Deserialize<UserDto.UserDetails>(jsonResponse, jsonSerializerOptions);
    }

    public async Task<bool> UpdateUserAsync(UserDto.UpdateUser userDetails)
    {
        var response = await httpClient.PutAsJsonAsync($"user/{userDetails.Id}", userDetails);
        return response.IsSuccessStatusCode;
    }

    // public async Task<IEnumerable<UserDto.Auth0User>> GetAuth0Users()
    // {
    //     var users = await httpClient.GetFromJsonAsync<IEnumerable<UserDto.Auth0User>>("user/auth/users");
    //     return users!;
    // }
    public async Task<IEnumerable<UserDto.Auth0User>> GetAuth0Users()
    {
        try
        {
            // Make the HTTP request and get the response
            var response = await httpClient.GetAsync("user/auth/users");

            // Check if the response indicates success
            if (response.IsSuccessStatusCode)
            {
                // Deserialize and return the users if the request was successful
                var users = await response.Content.ReadFromJsonAsync<IEnumerable<UserDto.Auth0User>>();
                return users ?? Enumerable.Empty<UserDto.Auth0User>(); // Ensure non-null result
            }
            else
            {
                // Handle various HTTP errors based on status code
                switch (response.StatusCode)
                {
                    case System.Net.HttpStatusCode.NotFound:
                        // Handle 404 Not Found case
                        Console.WriteLine("No users found.");
                        break;
                    case System.Net.HttpStatusCode.ServiceUnavailable:
                        // Handle 503 Service Unavailable case
                        Console.WriteLine("Auth0 service is unavailable. Please try again later.");
                        break;
                    case System.Net.HttpStatusCode.InternalServerError:
                        // Handle 500 Internal Server Error case
                        Console.WriteLine("An unexpected server error occurred.");
                        break;
                    default:
                        // Handle other status codes
                        Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                        break;
                }

                // Optionally, return an empty list if there's an error
                return Enumerable.Empty<UserDto.Auth0User>();
            }
        }
        catch (HttpRequestException ex)
        {
            // Handle network-related errors
            Console.WriteLine($"Network error: {ex.Message}");
            return Enumerable.Empty<UserDto.Auth0User>();
        }
        catch (Exception ex)
        {
            // Handle any other unexpected errors
            Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            return Enumerable.Empty<UserDto.Auth0User>();
        }
    }

    public async Task<bool> IsEmailTakenAsync(String email)
    {
        try
        {
            // Send a GET request to the backend with the email parameter
            var response = await httpClient.GetAsync($"user/exists?email={Uri.EscapeDataString(email)}");

            // If the response is successful, parse the result as a boolean
            if (response.IsSuccessStatusCode)
            {
                var result =  await response.Content.ReadFromJsonAsync<bool>();
                return result;
            }

            // Handle error or unexpected response status
            Console.WriteLine("Failed to check email availability.");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return false;
        }
    }
}

