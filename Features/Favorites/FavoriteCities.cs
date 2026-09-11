namespace weather_app.Features.Favorites;

public sealed class FavoriteCities
{
    public const int MaximumCount = 5;

    private readonly List<FavoriteCity> cities = [];

    public IReadOnlyList<FavoriteCity> Items => cities;

    public bool Contains(FavoriteCity city) => cities.Any(existing => existing.HasSameIdentity(city));

    public bool TryAdd(FavoriteCity city)
    {
        ArgumentNullException.ThrowIfNull(city);

        if (string.IsNullOrWhiteSpace(city.Name) || string.IsNullOrWhiteSpace(city.Query) ||
            cities.Count >= MaximumCount || Contains(city))
        {
            return false;
        }

        cities.Add(city);
        return true;
    }

    public bool Remove(FavoriteCity city)
    {
        ArgumentNullException.ThrowIfNull(city);

        var existing = cities.FirstOrDefault(favorite => favorite.HasSameIdentity(city));
        return existing is not null && cities.Remove(existing);
    }

    public void Replace(IEnumerable<FavoriteCity> favorites)
    {
        ArgumentNullException.ThrowIfNull(favorites);

        cities.Clear();
        foreach (var favorite in favorites)
        {
            TryAdd(favorite);
        }
    }
}
