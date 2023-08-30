using System.Globalization;

namespace RestApiLocalization;

public interface ICultureProvider
{
    /// <summary>Get the default culture name</summary>
    string? DefaultCultureName { get; }

    /// <summary>Get the current culture</summary>
    CultureInfo CurrentCulture { get; }

    /// <summary>Get the current UI culture</summary>
    CultureInfo CurrentUICulture { get; }

    /// <summary>Set the current application und UI culture</summary>
    void SetCurrentCulture(string cultureName);

    /// <summary>Get the culture by name</summary>
    /// <param name="cultureName">The culture name</param>
    CultureInfo? GetCulture(string cultureName);

    /// <summary>Get the supported cultures</summary>
    IList<CultureInfo> GetSupportedCultures();

    /// <summary>Get the supported culture descriptions</summary>
    IList<CultureDescription> GetSupportedCultureDescriptions();
}