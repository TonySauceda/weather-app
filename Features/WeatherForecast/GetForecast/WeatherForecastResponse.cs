namespace weather_app.Features.WeatherForecast.GetForecast;

public sealed record WeatherForecastResponse(
    string City,
    CurrentWeatherResponse Current,
    IReadOnlyList<DailyForecastResponse> UpcomingDays);
