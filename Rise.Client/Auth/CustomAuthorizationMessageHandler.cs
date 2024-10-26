namespace Rise.Client.Auth;

using Microsoft.JSInterop;
using IJSRuntime = Microsoft.JSInterop.IJSRuntime;
public class CustomAuthorizationMessageHandler : DelegatingHandler
{
    private readonly IJSRuntime _js;

    public CustomAuthorizationMessageHandler(IJSRuntime js)
    {
        _js = js;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _js.InvokeAsync<string>("sessionStorage.getItem", "access_token");
        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
        return await base.SendAsync(request, cancellationToken);
    }
}
