using weather_app.Features.WeatherForecast.SearchCities;

namespace weather_app.Features.Favorites;

public sealed record FavoriteCity(
    string Name,
    string Query,
    double? Latitude = null,
    double? Longitude = null)
{
    public static FavoriteCity FromCitySearchResult(CitySearchResult city) => new(
        city.Name,
        city.DisplayName,
        city.Latitude,
        city.Longitude);

    public static FavoriteCity FromForecastCity(string city)
    {
        var name = city.Split(',', 2, StringSplitOptions.TrimEntries)[0];

        return new FavoriteCity(name, city);
    }

    public bool HasSameIdentity(FavoriteCity other)
    {
        ArgumentNullException.ThrowIfNull(other);

        return Latitude is { } latitude && Longitude is { } longitude &&
               other.Latitude is { } otherLatitude && other.Longitude is { } otherLongitude
            ? latitude.Equals(otherLatitude) && longitude.Equals(otherLongitude)
            : string.Equals(Query, other.Query, StringComparison.OrdinalIgnoreCase);
    }

    public CitySearchResult? ToCitySearchResult() =>
        Latitude is { } latitude && Longitude is { } longitude
            ? new CitySearchResult(Query, null, null, latitude, longitude)
            : null;
}
