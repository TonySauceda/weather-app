namespace weather_app.Features.WeatherForecast.GetForecast;

public sealed record CurrentWeatherResponse(
    DateOnly Date,
    double TemperatureC,
    double FeelsLikeC,
    int HumidityPercentage,
    double WindSpeedKph,
    string Summary);
