using weather_app.Features.WeatherForecast.GetForecast;

namespace weather_app.Tests.Features.WeatherForecast.GetForecast;

public sealed class FakeOpenMeteoClient(WeatherForecastResponse forecast) : IOpenMeteoClient
{
    public int RequestCount { get; private set; }

    public Task<WeatherForecastResponse> GetForecastAsync(string city, CancellationToken cancellationToken)
    {
        RequestCount++;

        return Task.FromResult(forecast);
    }
}
