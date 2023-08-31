using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace RestApiLocalization;

/// <summary>Extensions for <see cref="IServiceCollection"/></summary>
public static class ServiceCollectionExtensions
{
    /// <summary>Add localization services</summary>
    /// <param name="serviceCollection">The service collection</param>
    /// <param name="cultureScope">The culture scope</param>
    /// <param name="defaultCulture">The default culture</param>
    /// <param name="supportedCultures">The supported cultures</param>
    /// <returns>The string without suffix</returns>
    public static IServiceCollection AddLocalization(this IServiceCollection serviceCollection,
        CultureScope? cultureScope = null, string? defaultCulture = null,
        IEnumerable<string>? supportedCultures = null)
    {
        // culture provider
        var cultureProvider = new CultureProvider(
            supportedCultures: supportedCultures ?? new List<string>(),
            defaultCultureName: defaultCulture,
            cultureScope: cultureScope ?? new());
        serviceCollection.AddSingleton<ICultureProvider>(cultureProvider);
        return serviceCollection;
    }

    /// <summary>Add localization services</summary>
    /// <param name="serviceCollection">The service collection</param>
    /// <param name="cultureScope">The culture scope</param>
    /// <param name="defaultCulture">The default culture</param>
    /// <param name="supportedCultures">The supported cultures</param>
    /// <returns>The string without suffix</returns>
    public static IServiceCollection AddLocalizationWithRequest(this IServiceCollection serviceCollection,
        CultureScope? cultureScope = null, string? defaultCulture = null,
        IEnumerable<string>? supportedCultures = null)
    {
        // culture provider
        var cultureProvider = new CultureProvider(
            supportedCultures: supportedCultures ?? new List<string>(),
            defaultCultureName: defaultCulture,
            cultureScope: cultureScope ?? new());
        serviceCollection.AddSingleton<ICultureProvider>(cultureProvider);

        // request localization
        serviceCollection.Configure<RequestLocalizationOptions>(options =>
        {
            if (!string.IsNullOrWhiteSpace(cultureProvider.DefaultCultureName))
            {
                options.DefaultRequestCulture =
                    new Microsoft.AspNetCore.Localization.RequestCulture(cultureProvider.DefaultCultureName);
            }
            options.SupportedCultures = cultureProvider.GetSupportedCultures();
        });
        return serviceCollection;
    }
}