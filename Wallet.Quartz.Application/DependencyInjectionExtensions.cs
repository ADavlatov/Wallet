using Microsoft.Extensions.DependencyInjection;
using Wallet.Quartz.Application.Services;
using Wallet.Quartz.Domain.Interfaces;

namespace Wallet.Quartz.Application;

public static class DependencyInjectionExtensions
{
    public static void ConfigureDependencies(this IServiceCollection services)
    {
        services.AddTransient<INotificationsService, NotificationsService>();
    }
}