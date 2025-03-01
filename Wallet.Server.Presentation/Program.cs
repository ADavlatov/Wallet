using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Wallet.Server.Infrastructure.Contexts;
using Wallet.Server.Infrastructure.Options;
using Wallet.Server.Presentation;

var builder = WebApplication.CreateBuilder(args);

Wallet.Server.Infrastructure.DependencyInjectionExtensions.ConfigureDependencies(builder.Services);
Wallet.Server.Application.DependencyInjectionExtensions.ConfigureDependencies(builder.Services);

builder.Services.AddDbContext<WalletContext>();
builder.Services.AddControllers(x => x.Filters.Add<GlobalExceptionFilter>());
builder.Services.AddEndpointsApiExplorer();

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.Section));

builder.Services.AddSwaggerGen(x =>
{
    x.SwaggerDoc("v1", new OpenApiInfo { Title = "Wallet", Version = "v1" });
    // var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    // x.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,          
            ValidateAudience = true,       
            ValidateLifetime = true,     
            ValidateIssuerSigningKey = true, 
            ValidIssuer = builder.Configuration["JwtOptions:Issuer"],      
            ValidAudience = builder.Configuration["JwtOptions:Audience"],     
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtOptions:SecretKey"]!))
        };
    });


var app = builder.Build();

app.UseRouting();
app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints(endpoints => endpoints.MapControllers());

app.Run();