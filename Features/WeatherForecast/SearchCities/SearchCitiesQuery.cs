using weather_app.Application.Mediator;

namespace weather_app.Features.WeatherForecast.SearchCities;

public sealed record SearchCitiesQuery(string SearchText) : IQuery<IReadOnlyList<CitySearchResult>>;
