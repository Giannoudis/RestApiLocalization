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
        // use AddLocalizationWithRequest() to register the request localization
        builder.Services.AddLocalization(
            cultureScope: new CultureScope(
                neutral: true,
                specific: true,
                installed: true,
                custom: false,
                replacement: false),
            supportedCultures: new[]
                {
                    "en", "en-US", "en-GB",
                    "de", "de-DE", "de-AT", "de-CH",
                    "zh",
                },
            defaultCulture: "en-US");

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
}