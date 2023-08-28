using Microsoft.AspNetCore.Components;
using RestApiLocalization.WebApp.Service;

namespace RestApiLocalization.WebApp.Pages;

public partial class Cultures
{
    [Inject] public CultureService? CultureService { get; set; }
    private List<CultureDescription>? cultures;
    private string? ErrorMessage { get; set; }

    private async Task SetupCultures()
    {
        try
        {
            if (CultureService != null)
            {
                cultures = await CultureService.GetAvailableCultureDescriptionsAsync();
            }
        }
        catch (Exception exception)
        {
            ErrorMessage = exception.GetBaseException().Message;
        }
    }

    protected override async Task OnInitializedAsync()
    {
        await SetupCultures();
        await base.OnInitializedAsync();
    }
}