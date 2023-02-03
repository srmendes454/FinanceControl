using Microsoft.Extensions.DependencyInjection;
using System;

namespace FinanceControl.Extensions.DependencyInjection;

public static class FinanceControlDependencyInjectionRepository
{
    public static void ConfigureRepositoriesDependencyInjection(this IServiceCollection services)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
    }
}