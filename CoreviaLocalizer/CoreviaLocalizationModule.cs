using Corevia.Dependency.Module;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;

namespace Corevia.Localization
{
    public class CoreviaLocalizationModule : CoreviaModule
    {
        public override void PreConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services
                .Replace(ServiceDescriptor.Singleton<IStringLocalizerFactory, JsonStringLocalizerFactory>())
                .Replace(ServiceDescriptor.Scoped<IStringLocalizer, JsonStringLocalizer>())
                .AddSingleton<IStringLocalizerFactory, JsonStringLocalizerFactory>()
                .AddSingleton<IJsonStringLocalizerFactory, JsonStringLocalizerFactory>()
                .AddScoped(typeof(IStringLocalizer<>), typeof(JsonStringLocalizer<>));

        }
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<LocalizationResourceOptions>(options =>
            {
                options.AddResource<DefaultResources>();
            });
        }
    }
}