namespace weather_app.Features.WeatherForecast.GetForecast;

public sealed record DailyForecastResponse(
    DateOnly Date,
    double MinimumTemperatureC,
    double MaximumTemperatureC,
    string Summary,
    int PrecipitationProbabilityPercentage,
    double PrecipitationMillimeters);
