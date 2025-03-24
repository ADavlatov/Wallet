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
builder.Services.AddScoped<RefreshTokenHandler>();

builder.Services.AddHttpClient("MainClient", client => { client.BaseAddress = new Uri("http://localhost:5221"); })
    .AddHttpMessageHandler<RefreshTokenHandler>();

var host = builder.Build();

await host.RunAsync();