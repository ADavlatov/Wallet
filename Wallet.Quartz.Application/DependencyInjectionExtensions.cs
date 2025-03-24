using Microsoft.Extensions.DependencyInjection;
using Wallet.Quartz.Application.Services;
using Wallet.Quartz.Domain.Interfaces;
using Wallet.Quartz.Infrastructure.Senders;

namespace Wallet.Quartz.Application;

public static class DependencyInjectionExtensions
{
    public static void AddApplicationLayerServices(this IServiceCollection services)
    {
        services.AddScoped<TelegramMessageSender>();
        services.AddTransient<INotificationsService, NotificationsService>();
    }
}