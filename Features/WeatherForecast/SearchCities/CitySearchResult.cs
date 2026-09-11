namespace weather_app.Features.WeatherForecast.SearchCities;

public sealed record CitySearchResult(
    string Name,
    string? AdministrativeArea,
    string? Country,
    double Latitude,
    double Longitude)
{
    public string DisplayName => string.Join(
        ", ",
        new[] { Name, AdministrativeArea, Country }
            .Where(value => !string.IsNullOrWhiteSpace(value)));
}
