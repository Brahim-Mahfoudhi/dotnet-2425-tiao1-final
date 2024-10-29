using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;

namespace Rise.Client.Users
{
    public partial class EmbeddedLogin
    {
        [Inject] private HttpClient HttpClient { get; set; } = default!;
        [Inject] private IJSRuntime Js { get; set; } = default!;
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;
        [Inject] private AuthenticationStateProvider AuthStateProvider { get; set; } = default!;
        [Inject] private IStringLocalizer<Login> Localizer { get; set; } = default!;
        [Inject] private IConfiguration Config { get; set; } = default!;

        private LoginModel loginModel = new LoginModel();
        private string? loginError;

        private void HandleCancelClick()
        {
            NavigationManager.NavigateTo("/");
        }

        private async void LoginAsAdmin()
        {
            loginModel.Email = "admin@hogent.be";
            loginModel.Password = "test";
            await HandleLogin();
        }

        private async Task HandleLogin()
        {
            try
            {
                // Retrieve configuration values from the Auth0Settings section
                var authority = Config["Auth0Settings:Authority"];
                var clientId = Config["Auth0Settings:ClientId"];
                var clientSecret = Config["Auth0Settings:ClientSecret"];
                var audience = Config["Auth0Settings:Audience"];
                var tokenUri = Config["Auth0Settings:TokenUri"];

                // Prepare the request payload for Auth0's token endpoint
                var loginRequest = new
                {
                    grant_type = "password",
                    client_id = clientId,
                    client_secret = clientSecret,
                    username = loginModel.Email,
                    password = loginModel.Password,
                    audience = audience, // If using a custom API
                    scope = "openid profile email"
                };

                var jsonRequest = JsonSerializer.Serialize(loginRequest);
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                // Send the login request
                var response = await HttpClient.PostAsync(tokenUri, content);
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var authResult = JsonSerializer.Deserialize<AuthResponse>(jsonResponse);

                    var accessToken = authResult?.access_token;

                    if (accessToken != null)
                    {
                        // Store access token (in session storage or any preferred storage)
                        await Js.InvokeVoidAsync("sessionStorage.setItem", "access_token", accessToken);

                        // Notify the authentication state provider
                        if (AuthStateProvider is CustomAuthStateProvider customAuthStateProvider)
                        {
                            customAuthStateProvider.NotifyUserAuthentication(accessToken);
                        }
                        loginError = null;
                        // Redirect or update authentication state
                        NavigationManager.NavigateTo("/");
                    }
                }
                else
                {
                    loginError = Localizer["WrongCreds"];
                }
            }
            catch (Exception ex)
            {
                loginError = $"An error occurred: {ex.Message}";
            }
        }

        public class LoginModel
        {
            [Required(ErrorMessage = "Email is required")]
            [EmailAddress(ErrorMessage = "Invalid email address")]
            public string Email { get; set; } = string.Empty;
            [Required(ErrorMessage = "Password is required")]
            public string Password { get; set; } = string.Empty;
        }

        public class AuthResponse
        {
            public string access_token { get; set; } = string.Empty;
            public string id_token { get; set; } = string.Empty;
            public int expires_in { get; set; }
            public string token_type { get; set; } = string.Empty;
        }
    }
}

