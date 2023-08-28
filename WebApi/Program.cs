using Microsoft.OpenApi.Models;

namespace RestApiLocalization.WebApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();

        // swagger
        builder.Services.AddSwaggerGen(setupAction =>
        {
            setupAction.SwaggerDoc("v1",
                new OpenApiInfo
                {
                    Title = "REST API Localization for .NET",
                    Version = "v1"
                });
        });

        // localization
        SetupLocalization(builder.Services);

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(setupAction => setupAction.DisplayOperationId());
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }

    private static void SetupLocalization(IServiceCollection services)
    {
        // culture
        var scope = new CultureScope(
            neutral: true,
            specific: true,
            installed: true,
            custom: false,
            replacement: false);
        string[] supportedCultures =
        {
            "en", "en-US", "en-GB",
            "de", "de-DE", "de-AT", "de-CH",
            "zh",
        };
        var cultureProvider = new CultureProvider(
            supportedCultures: supportedCultures,
            defaultCultureName: "en-US",
            cultureScope: scope);
        services.AddSingleton<ICultureProvider>(cultureProvider);

        // request localization
        services.Configure<RequestLocalizationOptions>(options =>
        {
            options.DefaultRequestCulture =
                new Microsoft.AspNetCore.Localization.RequestCulture(cultureProvider.DefaultCultureName);
            options.SupportedCultures = cultureProvider.GetSupportedCultures();
        });
    }
}