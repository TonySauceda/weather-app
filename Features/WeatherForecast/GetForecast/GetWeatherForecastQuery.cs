using weather_app.Application.Mediator;
using weather_app.Features.WeatherForecast.SearchCities;

namespace weather_app.Features.WeatherForecast.GetForecast;

public sealed record GetWeatherForecastQuery(
    string City,
    CitySearchResult? SelectedCity = null) : IQuery<WeatherForecastResponse>;
