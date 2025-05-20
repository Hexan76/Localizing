using System.Collections.Generic;
using System.Text.Json;

namespace HashtApp.Soft.Client;

public static class JsonFlattener
{
    public static Dictionary<string, string> FlattenJson(JsonElement jsonElement, string parentKey = "")
    {
        var flatDictionary = new Dictionary<string, string>();

        foreach (var property in jsonElement.EnumerateObject())
        {
            var key = string.IsNullOrEmpty(parentKey) ? property.Name : $"{parentKey}.{property.Name}";
            var value = property.Value;

            if (value.ValueKind == JsonValueKind.Object)
            {
                var nestedFlat = FlattenJson(value, key);
                foreach (var kvp in nestedFlat)
                    flatDictionary[kvp.Key] = kvp.Value;
            }
            else
            {
                flatDictionary[key] = value.ToString();
            }
        }

        return flatDictionary;
    }
}
