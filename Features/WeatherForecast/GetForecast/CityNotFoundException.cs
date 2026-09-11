namespace weather_app.Features.WeatherForecast.GetForecast;

public sealed class CityNotFoundException(string city)
    : Exception($"No se encontró la ciudad '{city}'.")
{
}
