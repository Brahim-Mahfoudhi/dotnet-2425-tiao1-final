using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Web;
using System.Globalization;
using Microsoft.JSInterop;
using Rise.Client;
using Rise.Client.Products;
using Rise.Shared.Products;
using Rise.Shared.Users;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Rise.Client.Auth;
using UserService = Rise.Client.Users.UserService;
using Microsoft.AspNetCore.Components.Authorization;



var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddAuthorizationCore(); 
// Register the custom AuthenticationStateProvider
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
// builder.Services.AddCascadingAuthenticationState();
// builder.Services.AddOidcAuthentication(options =>
// {
//     builder.Configuration.Bind("Auth0", options.ProviderOptions);
//     options.ProviderOptions.ResponseType = "code";
//     options.ProviderOptions.PostLogoutRedirectUri = builder.HostEnvironment.BaseAddress;
//     options.ProviderOptions.AdditionalProviderParameters.Add("audience", builder.Configuration["Auth0:Audience"]!);
// }).AddAccountClaimsPrincipalFactory<ArrayClaimsPrincipalFactory<RemoteUserAccount>>();


builder.Services.AddLocalization(Options => Options.ResourcesPath = "Resources.Labels");

// Register CustomAuthorizationMessageHandler for requests that need authorization
builder.Services.AddScoped<CustomAuthorizationMessageHandler>();

builder.Services.AddHttpClient<IProductService, ProductService>(client =>
{
    client.BaseAddress = new Uri($"{builder.HostEnvironment.BaseAddress}api/"); 
});

builder.Services.AddHttpClient<IUserService, UserService>(client =>
{
    client.BaseAddress = new Uri($"{builder.HostEnvironment.BaseAddress}api/");
}).AddHttpMessageHandler<CustomAuthorizationMessageHandler>();

var host = builder.Build();


// Set the culture
var jsInterop = host.Services.GetRequiredService<IJSRuntime>();
var result = await jsInterop.InvokeAsync<string>("blazorCulture.get");

// If no culture is found in the browser, set a default (e.g., "en-US").
var culture = new CultureInfo(result ?? "en-US");

CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

await host.RunAsync();
