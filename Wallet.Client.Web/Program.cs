using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Wallet.Client.Web;
using Wallet.Client.Web.Interfaces;
using Wallet.Client.Web.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddBlazorBootstrap();

builder.Services.AddScoped<ILocalStorageService, LocalStorageService>();

builder.Services.AddScoped(x => {
    var apiUrl = new Uri("http://localhost:5221");
    var httpClient = new HttpClient { BaseAddress = apiUrl };
    return httpClient;
});

var host = builder.Build();

await host.RunAsync();