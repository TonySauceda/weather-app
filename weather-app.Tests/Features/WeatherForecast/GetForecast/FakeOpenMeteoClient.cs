using weather_app.Features.WeatherForecast.GetForecast;
using weather_app.Features.WeatherForecast.SearchCities;

namespace weather_app.Tests.Features.WeatherForecast.GetForecast;

public sealed class FakeOpenMeteoClient(
    WeatherForecastResponse forecast,
    IReadOnlyList<CitySearchResult>? searchResults = null) : IOpenMeteoClient
{
    public int RequestCount { get; private set; }

    public int SearchRequestCount { get; private set; }

    public CitySearchResult? LastSelectedCity { get; private set; }

    public string? LastSearchText { get; private set; }

    public Task<WeatherForecastResponse> GetForecastAsync(string city, CancellationToken cancellationToken)
    {
        RequestCount++;

        return Task.FromResult(forecast);
    }

    public Task<WeatherForecastResponse> GetForecastAsync(CitySearchResult city, CancellationToken cancellationToken)
    {
        RequestCount++;
        LastSelectedCity = city;

        return Task.FromResult(forecast);
    }

    public Task<IReadOnlyList<CitySearchResult>> SearchCitiesAsync(string searchText, CancellationToken cancellationToken)
    {
        SearchRequestCount++;
        LastSearchText = searchText;

        return Task.FromResult(searchResults ?? (IReadOnlyList<CitySearchResult>)[]);
    }
}
