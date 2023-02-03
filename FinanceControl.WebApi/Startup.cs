using FinanceControl.Application.AutoMapper;
using FinanceControl.Application.Startup;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FinanceControl;

public class Startup : BaseStartup
{
    public Startup(IConfiguration configuration, IWebHostEnvironment env) : base(env: env, configuration: configuration,
        apiName: "finance-control")
    {

    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.FinanceControlAutoMapperConfiguration();
        BaseConfigureServices(services: services);
    }

    public void Configure(IApplicationBuilder app) => BaseConfigure(app);
}