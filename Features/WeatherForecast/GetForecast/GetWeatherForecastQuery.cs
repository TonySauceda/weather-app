using weather_app.Application.Mediator;

namespace weather_app.Features.WeatherForecast.GetForecast;

public sealed record GetWeatherForecastQuery(string City) : IQuery<WeatherForecastResponse>;
