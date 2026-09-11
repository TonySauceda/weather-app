namespace weather_app.Features.WeatherForecast.GetForecast;

public sealed record DailyForecastResponse(
    DateOnly Date,
    int MinimumTemperatureC,
    int MaximumTemperatureC,
    string Summary);
