using Authorization.Data.Entities;
using Authorization.Options;
using Microsoft.Extensions.Options;

namespace Authorization.Features.Weathers;

public class GetWeather:IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        
        var summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };
        // you can using IoptionsSnipet for scoped configuration options
        app.MapGet("/weatherforecast", (IOptions<HangFireOptions> hangFireOptions) =>
            {
                
                var forecast = Enumerable.Range(1, 5).Select(index =>
                        new WeatherForecast
                        (
                            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                            Random.Shared.Next(-20, 55),
                            summaries[Random.Shared.Next(summaries.Length)]
                        ))
                    .ToArray();
                return forecast;
            })
            .WithName("GetWeatherForecastNoAuthorized")
            .WithTags(Tags.Weathers)
            .MapToApiVersion(1);
        
        app.MapGet("/weatherforecast", () =>
            {
                var forecast = Enumerable.Range(1, 5).Select(index =>
                        new WeatherForecast
                        (
                            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                            Random.Shared.Next(-20, 55),
                            summaries[Random.Shared.Next(summaries.Length)]
                        ))
                    .ToArray();
                return forecast;
            })
            .WithName("GetWeatherForecastAuthorized")
            .WithTags(Tags.Weathers)
            .RequireAuthorization(PermissionEnum.ReadWeathers.Name,PermissionEnum.ReadUser.Name)
            .MapToApiVersion(2);
        

    }
    record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
    {
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }
}