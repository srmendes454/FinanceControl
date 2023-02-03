using Microsoft.Extensions.DependencyInjection;
using System;

namespace FinanceControl.Extensions.DependencyInjection;

public static class FinanceControlDependencyInjectionService
{
    public static void ConfigureServicesDependencyInjection(this IServiceCollection services)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
    }
}