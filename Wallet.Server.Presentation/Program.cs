using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Wallet.Server.Infrastructure.Contexts;
using Wallet.Server.Infrastructure.Options;
using Wallet.Server.Presentation;

var builder = WebApplication.CreateBuilder(args);

Wallet.Server.Infrastructure.DependencyInjectionExtensions.ConfigureDependencies(builder.Services);
Wallet.Server.Application.DependencyInjectionExtensions.ConfigureDependencies(builder.Services);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("https://localhost:7129")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});


builder.Services.AddDbContext<WalletContext>(o => o.UseSqlite("Data Source=wallet.db"));
builder.Services.AddControllers(x => x.Filters.Add<GlobalExceptionFilter>());
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpClient();
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.Section));
builder.Services.Configure<UrlOptions>(builder.Configuration.GetSection(UrlOptions.Section));

builder.Services.AddSwaggerGen(x =>
{
    x.SwaggerDoc("v1", new OpenApiInfo { Title = "Wallet", Version = "v1" });
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    x.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
    x.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });
    x.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtOptions = builder.Configuration.GetSection(JwtOptions.Section);
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtOptions["Issuer"],
            ValidAudience = jwtOptions["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions["SecretKey"]!))
        };
    });

var app = builder.Build();

app.UseCors();
app.UseRouting();
app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints(endpoints => endpoints.MapControllers());

app.Run();

//:TODO пофиксить обновление пароля
//:TODO почнинить переключение периодово на странице статистики
//:TODO выпилить хардкод
//:TODO добавить валидацию запросов
//:TODO добавить нормальную обработку ошибок
//:TODO добавить логгирование 
//:TODO добавить xml документацию
//:TODO поправить рефреш на клиенте