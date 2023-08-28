using MudBlazor.Services;
using RestApiLocalization.WebApp.Service;

namespace RestApiLocalization.WebApp;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddRazorPages();
        builder.Services.AddServerSideBlazor();

        // application services
        builder.Services.AddScoped(_ => new HttpClient 
        { 
            BaseAddress = new Uri("https://localhost:7246/") 
        });
        builder.Services.AddTransient<CultureService>();
        builder.Services.AddTransient<ProductService>();

        // mud blazor
        builder.Services.AddMudServices();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseStaticFiles();

        app.UseRouting();

        app.MapBlazorHub();
        app.MapFallbackToPage("/_Host");

        app.Run();
    }
}