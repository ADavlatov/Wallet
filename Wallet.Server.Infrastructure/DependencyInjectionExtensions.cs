using Microsoft.Extensions.DependencyInjection;
using Wallet.Server.Domain.Interfaces;
using Wallet.Server.Infrastructure.Repositories;

namespace Wallet.Server.Infrastructure;

public static class DependencyInjectionExtensions
{
    public static void ConfigureDependencies(this IServiceCollection services)
    {
        services.AddTransient<ICategoriesRepository, CategoriesRepository>();
        services.AddTransient<IGoalsRepository, GoalsRepository>();
        services.AddTransient<ITransactionsRepository, TransactionsRepository>();
        services.AddTransient<IUsersRepository, UsersRepository>();
    }
}