using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly IJSRuntime _js;
    private readonly HttpClient _httpClient;
    private ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());

    public CustomAuthStateProvider(IJSRuntime js, HttpClient httpClient)
    {
        _js = js;
        _httpClient = httpClient;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        // Check if the access token exists in session storage
        var token = await _js.InvokeAsync<string>("sessionStorage.getItem", "access_token");
        if (string.IsNullOrEmpty(token))
        {
            // Return anonymous state if no token is found
            return new AuthenticationState(_anonymous);
        }

        // Create an identity from token claims
        var identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");
        var user = new ClaimsPrincipal(identity);

        // Fetch additional user details and parse them
        var userDetails = await GetUserDetailsAsync(token);
        if (userDetails != null)
        {
            AddClaimsFromUserDetails(identity, userDetails);
        }

        return new AuthenticationState(user);
    }

    public async void NotifyUserAuthentication(string token)
    {
        var identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");
        var userDetails = await GetUserDetailsAsync(token);

        if (userDetails != null)
        {
            AddClaimsFromUserDetails(identity, userDetails);
        }

        var user = new ClaimsPrincipal(identity);
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }

    public void NotifyUserLogout()
    {
        var user = new ClaimsPrincipal(_anonymous);
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }

    private async Task<Dictionary<string, object>?> GetUserDetailsAsync(string token)
    {
        // Fetch user profile from /userinfo endpoint
        var request = new HttpRequestMessage(HttpMethod.Get, "https://dev-6yunsksn11owe71c.us.auth0.com/userinfo");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Dictionary<string, object>>(content);
        }

        return null;
    }

    private void AddClaimsFromUserDetails(ClaimsIdentity identity, Dictionary<string, object> userDetails)
    {
        foreach (var kvp in userDetails)
        {
            if (kvp.Value is JsonElement element && element.ValueKind == JsonValueKind.Array)
            {
                // Handle array claims (e.g., roles)
                foreach (var arrayItem in element.EnumerateArray())
                {
                    identity.AddClaim(new Claim(kvp.Key, arrayItem.ToString()));
                }
            }
            else
            {
                identity.AddClaim(new Claim(kvp.Key, kvp.Value.ToString() ?? string.Empty));
            }
        }
    }

    private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var payload = jwt.Split('.')[1];
        var jsonBytes = Convert.FromBase64String(AddPadding(payload));
        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

        var claims = new List<Claim>();
        if (keyValuePairs != null)
        {
            foreach (var kvp in keyValuePairs)
            {
                if (kvp.Value is JsonElement element && element.ValueKind == JsonValueKind.Array)
                {
                    // Handle array claims (e.g., roles)
                    foreach (var arrayItem in element.EnumerateArray())
                    {
                        claims.Add(new Claim(kvp.Key, arrayItem.ToString()));
                    }
                }
                else
                {
                    claims.Add(new Claim(kvp.Key, kvp.Value.ToString() ?? string.Empty));
                }
            }
        }
        return claims;
    }

    private static string AddPadding(string base64)
    {
        return base64.PadRight(base64.Length + (4 - base64.Length % 4) % 4, '=');
    }
}
