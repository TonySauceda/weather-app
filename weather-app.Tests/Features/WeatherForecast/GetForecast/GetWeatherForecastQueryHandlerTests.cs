using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Caching.Memory;
using weather_app.Features.WeatherForecast.GetForecast;

namespace weather_app.Tests.Features.WeatherForecast.GetForecast;

[TestClass]
public sealed class GetWeatherForecastQueryHandlerTests
{
    [TestMethod]
    public async Task HandleAsync_WithCity_ReturnsTheForecastFromTheWeatherProvider()
    {
        var weatherClient = new FakeOpenMeteoClient(CreateForecast());
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var handler = new GetWeatherForecastQueryHandler(weatherClient, cache);

        var forecast = await handler.HandleAsync(new GetWeatherForecastQuery(" Chihuahua "), CancellationToken.None);

        Assert.AreEqual("Chihuahua, Chihuahua, México", forecast.City);
        Assert.AreEqual(new DateOnly(2026, 9, 10), forecast.Current.Date);
        Assert.AreEqual(52, forecast.Current.HumidityPercentage);
        Assert.AreEqual(12.4, forecast.Current.WindSpeedKph);
        Assert.AreEqual(5, forecast.UpcomingDays.Count);
        Assert.AreEqual(new DateOnly(2026, 9, 11), forecast.UpcomingDays[0].Date);
        Assert.AreEqual(30, forecast.UpcomingDays[0].PrecipitationProbabilityPercentage);
        Assert.AreEqual(2.5, forecast.UpcomingDays[0].PrecipitationMillimeters);
        Assert.AreEqual(1, weatherClient.RequestCount);
    }

    [TestMethod]
    public async Task HandleAsync_WithRepeatedNormalizedCity_UsesTheCachedForecast()
    {
        var weatherClient = new FakeOpenMeteoClient(CreateForecast());
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var handler = new GetWeatherForecastQueryHandler(weatherClient, cache);

        var firstForecast = await handler.HandleAsync(new GetWeatherForecastQuery(" Chihuahua "), CancellationToken.None);
        var secondForecast = await handler.HandleAsync(new GetWeatherForecastQuery("chihuahua"), CancellationToken.None);

        Assert.AreSame(firstForecast, secondForecast);
        Assert.AreEqual(1, weatherClient.RequestCount);
    }

    [TestMethod]
    public async Task HandleAsync_WithBlankCity_ThrowsArgumentException()
    {
        var weatherClient = new FakeOpenMeteoClient(CreateForecast());
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var handler = new GetWeatherForecastQueryHandler(weatherClient, cache);

        await Assert.ThrowsExceptionAsync<ArgumentException>(
            () => handler.HandleAsync(new GetWeatherForecastQuery("   "), CancellationToken.None));

        Assert.AreEqual(0, weatherClient.RequestCount);
    }

    private static WeatherForecastResponse CreateForecast() => new(
        "Chihuahua, Chihuahua, México",
        new CurrentWeatherResponse(new DateOnly(2026, 9, 10), 24.3, 25.1, 52, 12.4, "Soleado"),
        [
            new DailyForecastResponse(new DateOnly(2026, 9, 11), 16, 31, "Parcialmente nublado", 30, 2.5),
            new DailyForecastResponse(new DateOnly(2026, 9, 12), 17, 29, "Nublado", 10, 0),
            new DailyForecastResponse(new DateOnly(2026, 9, 13), 16, 28, "Lluvioso", 65, 8.1),
            new DailyForecastResponse(new DateOnly(2026, 9, 14), 15, 30, "Soleado", 5, 0),
            new DailyForecastResponse(new DateOnly(2026, 9, 15), 16, 32, "Soleado", 0, 0)
        ]);
}
