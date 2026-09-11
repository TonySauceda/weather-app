namespace weather_app.Features.WeatherForecast.GetForecast;

public sealed record CurrentWeatherResponse(
    DateOnly Date,
    int TemperatureC,
    int FeelsLikeC,
    string Summary);
