using Microsoft.VisualStudio.TestTools.UnitTesting;
using weather_app.Features.WeatherForecast.GetForecast;
using weather_app.Features.WeatherForecast.SearchCities;
using weather_app.Tests.Features.WeatherForecast.GetForecast;

namespace weather_app.Tests.Features.WeatherForecast.SearchCities;

[TestClass]
public sealed class SearchCitiesQueryHandlerTests
{
    [TestMethod]
    public async Task HandleAsync_WithAtLeastTwoCharacters_ReturnsTrimmedSearchResults()
    {
        var expectedCity = new CitySearchResult("Chihuahua", "Chihuahua", "México", 28.6353, -106.0889);
        var weatherClient = new FakeOpenMeteoClient(CreateForecast(), [expectedCity]);
        var handler = new SearchCitiesQueryHandler(weatherClient);

        var results = await handler.HandleAsync(new SearchCitiesQuery(" Chihuahua "), CancellationToken.None);

        CollectionAssert.AreEqual(new[] { expectedCity }, results.ToArray());
        Assert.AreEqual("Chihuahua", weatherClient.LastSearchText);
        Assert.AreEqual(1, weatherClient.SearchRequestCount);
    }

    [TestMethod]
    public async Task HandleAsync_WithFewerThanTwoCharacters_DoesNotSearchTheProvider()
    {
        var weatherClient = new FakeOpenMeteoClient(CreateForecast());
        var handler = new SearchCitiesQueryHandler(weatherClient);

        var results = await handler.HandleAsync(new SearchCitiesQuery("M"), CancellationToken.None);

        Assert.AreEqual(0, results.Count);
        Assert.AreEqual(0, weatherClient.SearchRequestCount);
    }

    private static WeatherForecastResponse CreateForecast() => new(
        "Chihuahua, Chihuahua, México",
        new CurrentWeatherResponse(new DateOnly(2026, 9, 10), 24.3, 25.1, 52, 12.4, "Soleado"),
        []);
}
