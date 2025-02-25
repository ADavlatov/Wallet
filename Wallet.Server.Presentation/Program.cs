using System.Reflection;
using Microsoft.OpenApi.Models;
using Wallet.Server.Presentation;

var builder = WebApplication.CreateBuilder(args);

Wallet.Server.Infrastructure.DependencyInjectionExtensions.ConfigureDependencies(builder.Services);
Wallet.Server.Application.DependencyInjectionExtensions.ConfigureDependencies(builder.Services);

builder.Services.AddControllers(x => x.Filters.Add<GlobalExceptionFilter>());
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(x =>
{
    x.SwaggerDoc("v1", new OpenApiInfo { Title = "Wallet", Version = "v1" });
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    x.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

var app = builder.Build();

app.UseRouting();
app.UseEndpoints(endpoints => endpoints.MapControllers());

app.Run();