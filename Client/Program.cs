using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorApp.Client;
using Microsoft.Extensions.Configuration;
using System.Reflection;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

var configuration = new ConfigurationBuilder()
                                       .AddJsonStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("BlazorApp.Client.appsettings.json"))
                                       .Build();

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.Configuration["API_Prefix"] ?? builder.HostEnvironment.BaseAddress) });



//Supply HttpClient instances that include access tokens when making requests to the server project
//builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("BlazorStaticWebApps.ServerAPI"));

builder.Services.AddTransient(_ => { return configuration.GetSection("Settings").Get<Settings>(); });

builder.Services.AddMsalAuthentication(options =>
{
    builder.Configuration.Bind("AzureAd", options.ProviderOptions.Authentication);
    options.ProviderOptions.DefaultAccessTokenScopes.Add("api://e3e6fee9-1173-4620-8c69-d20a17c96a36/BlazorHostedAPI.Access");
});

await builder.Build().RunAsync();
