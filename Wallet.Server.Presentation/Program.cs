using Microsoft.EntityFrameworkCore;
using Wallet.Server.Application;
using Wallet.Server.Infrastructure.Contexts;
using Wallet.Server.Infrastructure.Options;
using Wallet.Server.Presentation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureDependencies();
builder.Services.ConfigureDependencies();

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

builder.Services.ConfigureSwagger();

builder.Services.ConfigureJwtAuthentication(builder.Configuration);

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
//:TODO почнинить переключение периодов на странице статистики
//:TODO добавить валидацию запросов
//:TODO добавить нормальную обработку ошибок
//:TODO добавить логгирование 
//:TODO поправить рефреш на клиенте
//:TODO убрать лишние поля в ответах