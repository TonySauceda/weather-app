using weather_app.Application.Mediator;
using Microsoft.Extensions.Caching.Memory;

namespace weather_app.Features.WeatherForecast.GetForecast;

public sealed class GetWeatherForecastQueryHandler(
    IOpenMeteoClient openMeteoClient,
    IMemoryCache cache) : IQueryHandler<GetWeatherForecastQuery, WeatherForecastResponse>
{
    public async Task<WeatherForecastResponse> HandleAsync(
        GetWeatherForecastQuery query,
        CancellationToken cancellationToken)
    {
        var city = query.City.Trim();
        if (string.IsNullOrWhiteSpace(city))
        {
            throw new ArgumentException("La ciudad es obligatoria.", nameof(query));
        }

        var cacheKey = $"weather-forecast:{city.ToUpperInvariant()}";
        if (cache.TryGetValue<WeatherForecastResponse>(cacheKey, out var cachedForecast))
        {
            return cachedForecast!;
        }

        var forecast = await openMeteoClient.GetForecastAsync(city, cancellationToken);
        cache.Set(cacheKey, forecast, TimeSpan.FromMinutes(10));

        return forecast;
    }
}
