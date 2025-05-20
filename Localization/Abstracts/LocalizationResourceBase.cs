namespace HashtApp.Soft.Client.Utilities.Localization.Abstracts;

public abstract class LocalizationResourceBase
{
    public string ResourceName { get; }

    protected LocalizationResourceBase(string resourceName)
    {
        ResourceName = resourceName;
    }
}

public class TypedLocalizationResource : LocalizationResourceBase
{
    public Type ResourceType { get; set; }
    public TypedLocalizationResource(string resourceName, Type type) : base(resourceName)
    {
        ResourceType = type;
    }
}

public class UnTypedLocalizationResource : LocalizationResourceBase
{
    public UnTypedLocalizationResource(string resourceName) : base(resourceName)
    {
    }
}