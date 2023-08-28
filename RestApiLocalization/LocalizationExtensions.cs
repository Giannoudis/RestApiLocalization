using System.Collections;

namespace RestApiLocalization;

public static class LocalizationExtensions
{
    /// <summary>
    /// Test if localization property exists
    /// </summary>
    /// <param name="type">The object type</param>
    /// <param name="propertyName">The property name</param>
    /// <returns>A hash set with ordinal ignore case</returns>
    public static bool IsLocalizable(this Type type, string propertyName)
    {
        if (string.IsNullOrWhiteSpace(propertyName))
        {
            throw new ArgumentException(nameof(propertyName));
        }
        return type.ContainsProperty(GetLocalizationsPropertyName(propertyName), typeof(Dictionary<string, string>));
    }

    /// <summary>
    /// Get the property localization values
    /// </summary>
    /// <param name="obj">The source object</param>
    /// <param name="propertyName">The property name</param>
    /// <returns>A dictionary with the object attributes</returns>
    public static  Dictionary<string, object?> GetLocalizations<TObject>(this TObject obj, string propertyName)
        where TObject : class
    {
        if (string.IsNullOrWhiteSpace(propertyName))
        {
            throw new ArgumentException(nameof(propertyName));
        }
        var localizationDictionary = obj.GetPropertyValue(GetLocalizationsPropertyName(propertyName)) as IDictionary;
        if (localizationDictionary == null)
        {
            return new();
        }

        Dictionary<string, object?> localizations = CastDict(localizationDictionary)
            .ToDictionary(entry => (string)entry.Key,
                entry => entry.Value);
        return localizations;
    }

    /// <summary>
    /// Get an optional localized property value
    /// </summary>
    /// <param name="obj">The source object</param>
    /// <param name="propertyName">The property name</param>
    /// <param name="culture">The culture name (default: current UI thread culture)</param>
    /// <returns>The localized value if present, otherwise the base property value</returns>
    public static object? GetOptionalLocalization<TObject>(this TObject obj, string propertyName,
        string? culture = null)
        where TObject : class
    {
        if (string.IsNullOrWhiteSpace(propertyName))
        {
            throw new ArgumentException(nameof(propertyName));
        }

        // source property
        var property = obj.GetType().GetProperty(GetLocalizationsPropertyName(propertyName));
        if (property == null)
        {
            throw new ArgumentException($"Unknown localization property {propertyName}");
        }

        Type? localizationType = null;
        if (property.PropertyType.IsGenericType &&
            property.PropertyType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
        {
            localizationType = property.PropertyType.GetGenericArguments()[1];
        }
        if (localizationType == null)
        {
            throw new ArgumentException($"Localization property must be a dictionary {propertyName}");
        }

        // test for base property
        var sourceType = obj.GetType();
        if (!sourceType.ContainsProperty(propertyName, localizationType))
        {
            throw new LocalizationException($"Type {sourceType} is missing property {propertyName}");
        }
        var baseValue = obj.GetPropertyValue(propertyName);
        if (baseValue == null)
        {
            return default;
        }

        return GetLocalizedValue(obj, propertyName, baseValue, culture);
    }

    /// <summary>
    /// Get an optional localized property value
    /// </summary>
    /// <param name="obj">The source object</param>
    /// <param name="propertyName">The property name</param>
    /// <param name="culture">The culture name (default: current UI thread culture)</param>
    /// <returns>The localized value if present, otherwise the base property value</returns>
    public static TValue? GetOptionalLocalization<TObject, TValue>(this TObject obj, string propertyName,
        string? culture = null)
        where TObject : class
    {
        if (string.IsNullOrWhiteSpace(propertyName))
        {
            throw new ArgumentException(nameof(propertyName));
        }

        // test for base property
        var sourceType = obj.GetType();
        if (!sourceType.ContainsProperty(propertyName, typeof(TValue)))
        {
            throw new LocalizationException($"Type {sourceType} is missing property {propertyName}");
        }
        var baseValue = obj.GetPropertyValue(propertyName);
        if (baseValue == null)
        {
            return default;
        }

        return GetLocalizedValue(obj, propertyName, (TValue)baseValue, culture);
    }

    /// <summary>
    /// Get the localized property value
    /// </summary>
    /// <param name="obj">The source object</param>
    /// <param name="propertyName">The property name</param>
    /// <param name="culture">The culture name (default: current UI thread culture)</param>
    /// <returns>The localized value if present, otherwise the base property value</returns>
    public static TValue GetLocalization<TObject, TValue>(this TObject obj, string propertyName, string? culture = null)
        where TObject : class
    {
        if (string.IsNullOrWhiteSpace(propertyName))
        {
            throw new ArgumentException(nameof(propertyName));
        }

        // test for base property
        var sourceType = obj.GetType();
        if (!sourceType.ContainsProperty(propertyName, typeof(TValue)))
        {
            throw new LocalizationException($"Type {sourceType} is missing property {propertyName}");
        }
        var baseValue = obj.GetPropertyValue(propertyName);
        if (baseValue == null)
        {
            throw new LocalizationException($"Type {sourceType} is missing value of property {propertyName}");
        }

        return GetLocalizedValue<TObject, TValue>(obj, propertyName, (TValue)baseValue, culture);
    }

    /// <summary>
    /// Map all source object localized values to the target object base properties
    /// </summary>
    /// <param name="target">The target object</param>
    /// <param name="source">The source object</param>
    /// <param name="culture">The culture name (default: current UI thread culture)</param>
    public static TTarget MapLocalizations<TTarget, TSource>(this TTarget target, TSource source,
        string? culture = null)
        where TSource : class
        where TTarget : class
    {
        var sourceProperties = source.GetType().GetObjectProperties();
        var targetProperties = target.GetType().GetObjectProperties();
        var localizationProperties = sourceProperties.Where(
            x => x.Name.EndsWith(Specification.LocalizationsPostfix));
        foreach (var localizationProperty in localizationProperties)
        {
            var propertyName = localizationProperty.Name.Replace(
                Specification.LocalizationsPostfix, string.Empty);
            var targetProperty = targetProperties.FirstOrDefault(x => string.Equals(x.Name, propertyName));
            if (targetProperty == null)
            {
                continue;
            }
            MapLocalization(target, source, propertyName, culture);
        }
        return target;
    }

    /// <summary>
    /// Map a source object localized value to the target object base property
    /// </summary>
    /// <param name="target">The target object</param>
    /// <param name="source">The source object</param>
    /// <param name="propertyName">The property name</param>
    /// <param name="culture">The culture name (default: current UI thread culture)</param>
    public static void MapLocalization<TTarget, TSource>(this TTarget target, TSource source,
        string propertyName, string? culture = null)
        where TTarget : class
        where TSource : class
    {
        if (string.IsNullOrWhiteSpace(propertyName))
        {
            throw new ArgumentException(nameof(propertyName));
        }

        // culture
        culture ??= Thread.CurrentThread.CurrentUICulture.Name;

        var localization = GetOptionalLocalization(source, propertyName, culture);
        if (localization != null)
        {
            target.SetPropertyValue(propertyName, localization);
        }
    }

    private static IEnumerable<DictionaryEntry> CastDict(IDictionary dictionary)
    {
        foreach (DictionaryEntry entry in dictionary)
        {
            yield return entry;
        }
    }

    private static TValue GetLocalizedValue<TObject, TValue>(TObject obj, string propertyName,
        TValue baseValue, string? culture)
        where TObject : class
    {
        var localizations = GetLocalizations(obj, propertyName);
        if (!localizations.Any())
        {
            return baseValue;
        }

        // culture
        culture ??= Thread.CurrentThread.CurrentUICulture.Name;
        culture = culture.Trim();

        // specific language localization (e.g. en-US)
        var cultureName = localizations.Keys.FirstOrDefault(
            x => string.Equals(x, culture, StringComparison.OrdinalIgnoreCase));
        if (!string.IsNullOrEmpty(cultureName))
        {
            var localizationValue = localizations[cultureName];
            if (localizationValue == null)
            {
                return baseValue;
            }
            return (TValue)localizationValue;
        }

        // neutral language
        var index = culture.IndexOf('-');
        if (index <= 0 && culture.Length == 2)
        {
            // search for first country specific language
            var specificCulture = localizations.Keys.FirstOrDefault(
                x => x.StartsWith(culture, StringComparison.OrdinalIgnoreCase));
            if (specificCulture == null)
            {
                return baseValue;
            }
            var localizationValue = localizations[specificCulture];
            if (localizationValue == null)
            {
                return baseValue;
            }
            return (TValue)localizationValue;
        }

        // neutral language localization (e.g. en, de)
        var neutralCulture = culture.Substring(0, index);
        cultureName = localizations.Keys.FirstOrDefault(
            x => string.Equals(x, neutralCulture, StringComparison.OrdinalIgnoreCase));
        if (!string.IsNullOrEmpty(cultureName))
        {
            var localizationValue = localizations[cultureName];
            if (localizationValue == null)
            {
                return baseValue;
            }
            return (TValue)localizationValue;
        }

        return baseValue;
    }

    private static string GetLocalizationsPropertyName(string propertyName) =>
        $"{propertyName}{Specification.LocalizationsPostfix}";
}