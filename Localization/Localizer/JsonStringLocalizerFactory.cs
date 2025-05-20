using HashtApp.Soft.Client.Utilities.Localization.Abstracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Text.Json;

namespace HashtApp.Soft.Client.Utilities;
public class JsonStringLocalizerFactory : IJsonStringLocalizerFactory
{
    private readonly Dictionary<Type, Dictionary<string, Dictionary<string, string>>> _localizedData = new();
    private readonly LocalizationResourceDictionary _cacheResources;
    private readonly SupportedCulturesLocaliztionOptions _cultures;
    public JsonStringLocalizerFactory(
        IOptions<SupportedCulturesLocaliztionOptions> cultures,
        IOptions<LocalizationResourceOptions> options,
        IServiceProvider serviceProvider)
    {
        _cacheResources = options.Value.Resources;
        _cultures = cultures.Value;
        PreLoadDictionary();
    }

    public IStringLocalizer Create(Type resourceSource)
    {
        if (!_localizedData.TryGetValue(resourceSource, out var cultureDict))
        {
            throw new InvalidOperationException($"No resource registered for {resourceSource.Name}");
        }

        return new JsonStringLocalizer(cultureDict);
    }

    public IStringLocalizer Create(string baseName, string location)
    {

        //TODO Handle EmbeddedResource and PhysicalResource by options
        var resourceType = _cacheResources.Values
            .OfType<TypedLocalizationResource>()
            .FirstOrDefault(resource => resource.ResourceName == baseName)
            ?.ResourceType;

        if (resourceType == null)
            throw new InvalidOperationException($"No resource type found for {location}.{baseName}");

        return Create(resourceType);
    }

    private void PreLoadDictionary()
    {
        foreach (var resource in _cacheResources.Values.OfType<TypedLocalizationResource>())
        {
            var assembly = resource.ResourceType.Assembly;
            var resourceNamePrefix = $"Resources.{resource.ResourceType.Name}.";

            var resourceFiles = assembly.GetManifestResourceNames()
                .Where(name => name.EndsWith(".json"));

            var perCultureDict = new Dictionary<string, Dictionary<string, string>>();

            foreach (var file in resourceFiles)
            {
                var culture = ExtractCulture(file);
                if (!_cultures.Cultures.TryGetValue(new CultureInfo(culture), out var supportCulture))
                {
                    continue;
                }
                using var stream = assembly.GetManifestResourceStream(file)!;
                using var reader = new StreamReader(stream);
                var json = reader.ReadToEnd();

                using var document = JsonDocument.Parse(json);
                var flat = JsonFlattener.FlattenJson(document.RootElement);
                perCultureDict[supportCulture.TwoLetterISOLanguageName] = flat;
            }

            _localizedData[resource.ResourceType] = perCultureDict;
        }
    }

    private static string ExtractCulture(string resourceName)
    {
        // Resources.MyResource.fa.json → fa
        var parts = resourceName.Split('.');
        return parts.Length >= 3 ? parts[^2] : "en"; // fallback به en اگه پیدا نشد
    }
}
public static class ExtensionLocalization
{
    public static IServiceCollection ReplaceLocalization(this IServiceCollection services)
    {
        return services
            .Replace(ServiceDescriptor.Singleton<IStringLocalizerFactory, JsonStringLocalizerFactory>())
            .Replace(ServiceDescriptor.Scoped<IStringLocalizer, JsonStringLocalizer>())
            .AddSingleton<IStringLocalizerFactory, JsonStringLocalizerFactory>()
            .AddSingleton<IJsonStringLocalizerFactory, JsonStringLocalizerFactory>()
            .AddScoped(typeof(IStringLocalizer<>), typeof(JsonStringLocalizer<>));
    }
}
