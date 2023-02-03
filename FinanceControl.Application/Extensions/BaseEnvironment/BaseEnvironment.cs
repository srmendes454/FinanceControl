using Microsoft.Extensions.Hosting;

namespace FinanceControl.Extensions.BaseEnvironment;

public static class BaseEnvironment
{
    public static bool IsDevelopmentEnviroment(this IHostEnvironment hostEnvironment)
    {
        if (hostEnvironment == null)
        {
            return true;
        }
        else if (string.IsNullOrEmpty(hostEnvironment.EnvironmentName))
        {
            return true;
        }
        else
        {
            return hostEnvironment.EnvironmentName.Equals("dev") || hostEnvironment.EnvironmentName.ToLower().Equals("development");
        }
    }
    public static bool IsProductionEnviroment(this IHostEnvironment hostEnvironment)
    {
        if (hostEnvironment == null)
        {
            return false;
        }
        else if (string.IsNullOrEmpty(hostEnvironment.EnvironmentName))
        {
            return false;
        }
        else
        {
            return hostEnvironment.EnvironmentName.Equals("prod") || hostEnvironment.EnvironmentName.ToLower().Equals("production"); ;
        }
    }
}