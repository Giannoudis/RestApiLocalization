using System.Globalization;
using Microsoft.AspNetCore.Mvc;

namespace RestApiLocalization.WebApi.Controllers;

[ApiController]
[Route("cultures")]
public class CulturesController : ControllerBase
{
    private ICultureProvider CultureProvider { get; }

    public CulturesController(ICultureProvider cultureProvider)
    {
        CultureProvider = cultureProvider;
    }

    /// <summary>Get available cultures</summary>
    [HttpGet(Name = "GetCultures")]
    public IEnumerable<string> GetCultures()
    {
        var cultures = CultureProvider.GetSupportedCultures()
            .Select(x => x.Name).ToList();
        return cultures;
    }

    /// <summary>Get available culture descriptions</summary>
    [HttpGet("description", Name = "GetCultureDescriptions")]
    public IEnumerable<CultureDescription> GetCultureDescriptions()
    {
        var cultures = CultureProvider.GetSupportedCultureDescriptions();
        return cultures;
    }

    /// <summary>Get system cultures</summary>
    [HttpGet("system", Name = "GetSystemCultures")]
    public IEnumerable<CultureDescription> GetSystemCultures(bool neutral = true,
        bool specific = true, bool installed = true, bool custom = false, bool replacement = false)
    {
        var scope = new CultureScope(neutral, specific, installed, custom, replacement);
        var cultures = CultureInfo.GetCultures(scope.GetCultureTypes())
            .Select(x => new CultureDescription(x));
        return cultures;
    }
}