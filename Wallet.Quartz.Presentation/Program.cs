using Microsoft.OpenApi.Models;
using Quartz;
using Quartz.AspNetCore;
using Wallet.Quartz.Infrastructure.Contexts;
using Wallet.Quartz.Infrastructure.Quartz;

var builder = WebApplication.CreateBuilder(args);

Wallet.Quartz.Infrastructure.DependencyInjectionExtensions.ConfigureDependencies(builder.Services);
Wallet.Quartz.Application.DependencyInjectionExtensions.ConfigureDependencies(builder.Services);

builder.Services.AddDbContext<QuartzContext>();

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

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddQuartz(q =>
{
    q.SchedulerId = "Notification-Scheduler";
    q.SchedulerName = "Notification Scheduler";

    q.AddJob<NotificationJob>(opts => {
        opts.WithIdentity("NotificationJob");
        opts.StoreDurably();
    });

});

builder.Services.AddScoped<QuartzNotificationScheduler>(); 

builder.Services.AddQuartzServer(options =>
{
    options.WaitForJobsToComplete = true;
});

var app = builder.Build();

app.UseCors();
app.UseRouting();
app.UseSwagger();
app.UseSwaggerUI();
app.UseEndpoints(endpoints => endpoints.MapControllers());

app.Run();