using System.Reflection;

namespace RestApiLocalization;

/// <summary>
/// Information about a c# object
/// </summary>
public static class ObjectInfo
{
    private static readonly Dictionary<Type, List<PropertyInfo>> Properties = new();
    private static readonly object Locker = new();

    /// <summary>
    /// Get object properties
    /// </summary>
    /// <remarks>Use the local type cache</remarks>
    /// <param name="type">The object type</param>
    /// <returns>The object property infos</returns>
    public static IList<PropertyInfo> GetObjectProperties(this Type type)
    {
        lock (Locker)
        {
            if (!Properties.ContainsKey(type))
            {
                Properties[type] = new(type.GetInstanceProperties());
            }
            return Properties[type];
        }
    }

    /// <summary>Get the public instance properties</summary>
    /// <param name="type">The type</param>
    /// <returns>The public type properties</returns>
    private static List<PropertyInfo> GetInstanceProperties(this Type type) =>
        type.GetProperties(BindingFlags.Public | BindingFlags.Instance).ToList();

    /// <summary>
    /// Get object property
    /// </summary>
    /// <param name="type">The object type</param>
    /// <param name="name">The property name</param>
    /// <returns>The property info</returns>
    public static PropertyInfo? GetProperty(this Type type, string name) =>
        GetObjectProperties(type).FirstOrDefault(x => string.Equals(x.Name, name));

    /// <summary>
    /// Test if property from a specific type exists
    /// </summary>
    /// <param name="type">The object type</param>
    /// <param name="name">The property name</param>
    /// <param name="propertyType">The property type</param>
    /// <returns>A hash set with ordinal ignore case</returns>
    public static bool ContainsProperty(this Type type, string name, Type propertyType)
    {
        var property = GetProperty(type, name);
        return property != null && propertyType == property.PropertyType;
    }

    /// <summary>
    /// Get object property value
    /// </summary>
    /// <param name="obj">The object</param>
    /// <param name="name">The property name</param>
    /// <param name="defaultValue">default value</param>
    /// <returns>The property value</returns>
    public static object? GetPropertyValue(this object obj,
        string name, object? defaultValue = default)
    {
        if (obj == null)
        {
            throw new ArgumentNullException(nameof(obj));
        }
        var property = GetProperty(obj.GetType(), name);
        var value = property?.GetValue(obj);
        if (value == null)
        {
            return defaultValue;
        }

        return value;
    }

    /// <summary>
    /// Set object property value
    /// </summary>
    /// <param name="obj">The object</param>
    /// <param name="name">The property name</param>
    /// <param name="value">The value to set</param>
    public static void SetPropertyValue(this object obj, string name, object value)
    {
        if (obj == null)
        {
            throw new ArgumentNullException(nameof(obj));
        }
        var property = GetProperty(obj.GetType(), name);
        if (property != null)
        {
            property.SetValue(obj, value);
        }
    }
}