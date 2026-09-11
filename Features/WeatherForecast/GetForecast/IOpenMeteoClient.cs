namespace weather_app.Features.WeatherForecast.GetForecast;

public interface IOpenMeteoClient
{
    Task<WeatherForecastResponse> GetForecastAsync(string city, CancellationToken cancellationToken);
}
