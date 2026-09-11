using weather_app.Features.WeatherForecast.SearchCities;

namespace weather_app.Features.WeatherForecast.GetForecast;

public interface IOpenMeteoClient
{
    Task<WeatherForecastResponse> GetForecastAsync(string city, CancellationToken cancellationToken);

    Task<WeatherForecastResponse> GetForecastAsync(CitySearchResult city, CancellationToken cancellationToken);

    Task<IReadOnlyList<CitySearchResult>> SearchCitiesAsync(string searchText, CancellationToken cancellationToken);
}
