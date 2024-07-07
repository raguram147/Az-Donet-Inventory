using System.Reflection;
using Serilog;

namespace InventoryManagement.Extensions
{
    public static class ServiceExtensions
    {
        public static void AutoRegisterServices(this IServiceCollection services, Assembly assembly)
        {
            var types = assembly.GetTypes();

            // Register services
            var serviceTypes = types.Where(t => t.Name.EndsWith("Service") && t.IsClass && !t.IsAbstract);
            foreach (var serviceType in serviceTypes)
            {
                var interfaceType = serviceType.GetInterfaces().FirstOrDefault(i => i.Name == "I" + serviceType.Name);
                if (interfaceType != null)
                {
                    services.AddScoped(interfaceType, serviceType);
                }
            }

            // Register repositories
            var repositoryTypes = types.Where(t => t.Name.EndsWith("Repository") && t.IsClass && !t.IsAbstract);
            foreach (var repositoryType in repositoryTypes)
            {
                var interfaceType = repositoryType.GetInterfaces().FirstOrDefault(i => i.Name == "I" + repositoryType.Name);
                if (interfaceType != null)
                {
                    services.AddScoped(interfaceType, repositoryType);
                }
            }
        }

        public static void ConfigureLogging(this IServiceCollection services)
        {
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddSerilog(dispose: true);  // Ensure Serilog is the only logging provider
            })
            .Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Information);
        }

        public static void ConfigureCaching(this IServiceCollection services)
        {
            services.AddMemoryCache();
        }
    }
}
