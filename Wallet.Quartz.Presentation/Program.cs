using Microsoft.OpenApi.Models;
using Wallet.Quartz.Infrastructure.Contexts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<QuartzContext>();
builder.Services.AddSwaggerGen(x =>
{
    x.SwaggerDoc("v1", new OpenApiInfo { Title = "Wallet", Version = "v1" });
    // var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    // x.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.Run();