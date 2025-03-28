using Microsoft.Extensions.DependencyInjection;
using Wallet.Server.Application.Services;
using Wallet.Server.Domain.Interfaces.Services;

namespace Wallet.Server.Application;

public static class DependencyInjectionExtensions
{
    public static void ConfigureDependencies(this IServiceCollection services)
    {
        services.AddTransient<ICategoriesService, CategoriesService>();
        services.AddTransient<IGoalsService, GoalsService>();
        services.AddTransient<ITransactionsService, TransactionsService>();
        services.AddTransient<IUsersService, UsersService>();
        services.AddTransient<IStatsService, StatsService>();
        services.AddTransient<INotificationsService, NotificationsService>();
    }
}