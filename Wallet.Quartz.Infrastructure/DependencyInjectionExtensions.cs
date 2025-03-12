using Microsoft.Extensions.DependencyInjection;
using Wallet.Quartz.Domain.Interfaces;
using Wallet.Quartz.Infrastructure.Notifications;
using Wallet.Quartz.Infrastructure.Quartz;
using Wallet.Quartz.Infrastructure.Repositories;

namespace Wallet.Quartz.Infrastructure;

public static class DependencyInjectionExtensions
{
    public static void ConfigureDependencies(this IServiceCollection services)
    {
        services.AddTransient<INotificationsRepository, NotificationsRepository>();
        services.AddTransient<INotificationSender, EmailNotificationSender>();
        services.AddTransient<QuartzNotificationScheduler>();
    }
}