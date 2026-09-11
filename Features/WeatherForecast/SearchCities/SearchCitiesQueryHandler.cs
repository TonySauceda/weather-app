using weather_app.Application.Mediator;
using weather_app.Features.WeatherForecast.GetForecast;

namespace weather_app.Features.WeatherForecast.SearchCities;

public sealed class SearchCitiesQueryHandler(
    IOpenMeteoClient openMeteoClient) : IQueryHandler<SearchCitiesQuery, IReadOnlyList<CitySearchResult>>
{
    public Task<IReadOnlyList<CitySearchResult>> HandleAsync(
        SearchCitiesQuery query,
        CancellationToken cancellationToken)
    {
        var searchText = query.SearchText.Trim();
        if (searchText.Length < 2)
        {
            return Task.FromResult<IReadOnlyList<CitySearchResult>>([]);
        }

        return openMeteoClient.SearchCitiesAsync(searchText, cancellationToken);
    }
}
