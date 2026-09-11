using Microsoft.VisualStudio.TestTools.UnitTesting;
using weather_app.Features.WeatherForecast.GetForecast;

namespace weather_app.Tests.Features.WeatherForecast.GetForecast;

[TestClass]
public sealed class GetWeatherForecastQueryHandlerTests
{
    [TestMethod]
    public async Task HandleAsync_WithCity_ReturnsCurrentWeatherAndFiveUpcomingDays()
    {
        var handler = new GetWeatherForecastQueryHandler();

        var forecast = await handler.HandleAsync(new GetWeatherForecastQuery(" Chihuahua "), CancellationToken.None);

        Assert.AreEqual("Chihuahua", forecast.City);
        Assert.AreEqual(DateOnly.FromDateTime(DateTime.Today), forecast.Current.Date);
        Assert.AreEqual(5, forecast.UpcomingDays.Count);
        Assert.AreEqual(forecast.Current.Date.AddDays(1), forecast.UpcomingDays[0].Date);
        Assert.AreEqual(forecast.Current.Date.AddDays(5), forecast.UpcomingDays[4].Date);
    }

    [TestMethod]
    public async Task HandleAsync_WithDifferentCity_ReturnsForecastForRequestedCity()
    {
        var handler = new GetWeatherForecastQueryHandler();

        var forecast = await handler.HandleAsync(new GetWeatherForecastQuery("Monterrey"), CancellationToken.None);

        Assert.AreEqual("Monterrey", forecast.City);
        Assert.IsFalse(string.IsNullOrWhiteSpace(forecast.Current.Summary));
    }

    [TestMethod]
    public async Task HandleAsync_WithBlankCity_ThrowsArgumentException()
    {
        var handler = new GetWeatherForecastQueryHandler();

        await Assert.ThrowsExceptionAsync<ArgumentException>(
            () => handler.HandleAsync(new GetWeatherForecastQuery("   "), CancellationToken.None));
    }
}
