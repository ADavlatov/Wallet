using Quartz;
using Quartz.AspNetCore;
using Wallet.Quartz.Application;
using Wallet.Quartz.Domain.Interfaces;
using Wallet.Quartz.Infrastructure;
using Wallet.Quartz.Infrastructure.Contexts;
using Wallet.Quartz.Infrastructure.Quartz;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructureLayerServices();
builder.Services.AddApplicationLayerServices();

builder.Services.AddDbContext<QuartzContext>();
builder.Services.AddHttpClient();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5221")
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

    q.AddJob<NotificationJob>(opts =>
    {
        opts.WithIdentity("NotificationJob");
        opts.StoreDurably();
    });
});

builder.Services.AddScoped<INotificationsScheduler, QuartzNotificationScheduler>();
builder.Services.AddQuartzServer(options => { options.WaitForJobsToComplete = true; });

var app = builder.Build();

app.UseCors();
app.UseRouting();
app.UseSwagger();
app.UseSwaggerUI();
app.UseEndpoints(endpoints => endpoints.MapControllers());

app.Run();