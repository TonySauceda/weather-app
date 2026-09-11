using Microsoft.VisualStudio.TestTools.UnitTesting;
using weather_app.Features.Favorites;

namespace weather_app.Tests.Features.Favorites;

[TestClass]
public sealed class FavoriteCitiesTests
{
    [TestMethod]
    public void TryAdd_WithFiveDistinctCities_AddsAllCities()
    {
        var favorites = new FavoriteCities();

        var added = Enumerable.Range(1, FavoriteCities.MaximumCount)
            .Select(index => favorites.TryAdd(CreateCity(index)))
            .ToArray();

        CollectionAssert.AreEqual(Enumerable.Repeat(true, FavoriteCities.MaximumCount).ToArray(), added);
        Assert.AreEqual(FavoriteCities.MaximumCount, favorites.Items.Count);
    }

    [TestMethod]
    public void TryAdd_WhenFiveCitiesAlreadyExist_DoesNotAddAnotherCity()
    {
        var favorites = new FavoriteCities();
        foreach (var index in Enumerable.Range(1, FavoriteCities.MaximumCount))
        {
            favorites.TryAdd(CreateCity(index));
        }

        var wasAdded = favorites.TryAdd(CreateCity(FavoriteCities.MaximumCount + 1));

        Assert.IsFalse(wasAdded);
        Assert.AreEqual(FavoriteCities.MaximumCount, favorites.Items.Count);
    }

    [TestMethod]
    public void TryAdd_WhenCityWithTheSameCoordinatesAlreadyExists_DoesNotDuplicateIt()
    {
        var favorites = new FavoriteCities();
        favorites.TryAdd(new FavoriteCity("Chihuahua", "Chihuahua, México", 28.6353, -106.0889));

        var wasAdded = favorites.TryAdd(new FavoriteCity("Chihuahua", "Chihuahua", 28.6353, -106.0889));

        Assert.IsFalse(wasAdded);
        Assert.AreEqual(1, favorites.Items.Count);
    }

    [TestMethod]
    public void Remove_WithAnExistingCity_RemovesItFromTheCollection()
    {
        var city = CreateCity(1);
        var favorites = new FavoriteCities();
        favorites.TryAdd(city);

        var wasRemoved = favorites.Remove(city);

        Assert.IsTrue(wasRemoved);
        Assert.AreEqual(0, favorites.Items.Count);
    }

    private static FavoriteCity CreateCity(int index) => new(
        $"Ciudad {index}",
        $"Ciudad {index}, México",
        index,
        -index);
}
