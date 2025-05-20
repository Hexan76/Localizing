using System.Globalization;

namespace HashtApp.Soft.Client.Utilities.Localization.Abstracts;

public class SupportedCulturesLocaliztionOptions
{
    public HashSet<CultureInfo> Cultures { get; } = new HashSet<CultureInfo>();

    public void AddCulture(string cultureName)
    {
        var culture = new CultureInfo(cultureName);
        Cultures.Add(culture);
    }
}
