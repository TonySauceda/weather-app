using Microsoft.VisualStudio.TestTools.UnitTesting;
using weather_app.Features.WeatherForecast.GetForecast;

namespace weather_app.Tests.Features.WeatherForecast.GetForecast;

[TestClass]
public sealed class OpenMeteoClientTests
{
    [TestMethod]
    public async Task GetForecastAsync_WithProviderResponses_MapsCurrentAndDailyWeather()
    {
        using var handler = new QueueHttpMessageHandler(
        [
            """
            { "results": [{ "name": "Chihuahua", "admin1": "Chihuahua", "country": "México", "latitude": 28.6353, "longitude": -106.0889 }] }
            """,
            """
            {
              "current": { "time": "2026-09-10T14:00", "temperature_2m": 24.3, "apparent_temperature": 25.1, "relative_humidity_2m": 52, "wind_speed_10m": 12.4, "weather_code": 0 },
              "daily": {
                "time": ["2026-09-10", "2026-09-11", "2026-09-12", "2026-09-13", "2026-09-14", "2026-09-15"],
                "weather_code": [0, 2, 3, 61, 0, 1],
                "temperature_2m_min": [15, 16, 17, 16, 15, 16],
                "temperature_2m_max": [30, 31, 29, 28, 30, 32],
                "precipitation_probability_max": [0, 30, 10, 65, 5, 0],
                "precipitation_sum": [0, 2.5, 0, 8.1, 0, 0]
              }
            }
            """
        ]);
        using var httpClient = new HttpClient(handler);
        var client = new OpenMeteoClient(httpClient);

        var forecast = await client.GetForecastAsync("Chihuahua", CancellationToken.None);

        Assert.AreEqual("Chihuahua, Chihuahua, México", forecast.City);
        Assert.AreEqual(24.3, forecast.Current.TemperatureC);
        Assert.AreEqual(52, forecast.Current.HumidityPercentage);
        Assert.AreEqual("Soleado", forecast.Current.Summary);
        Assert.AreEqual(5, forecast.UpcomingDays.Count);
        Assert.AreEqual(new DateOnly(2026, 9, 11), forecast.UpcomingDays[0].Date);
        Assert.AreEqual("Parcialmente nublado", forecast.UpcomingDays[0].Summary);
        Assert.AreEqual("Lluvioso", forecast.UpcomingDays[2].Summary);
        Assert.AreEqual(65, forecast.UpcomingDays[2].PrecipitationProbabilityPercentage);
        StringAssert.Contains(handler.RequestUris[1].Query, "forecast_days=6");
        StringAssert.Contains(handler.RequestUris[1].Query, "timezone=auto");
        Assert.IsFalse(handler.RequestUris[1].Query.Contains("hourly=", StringComparison.Ordinal));
    }

    [TestMethod]
    public async Task GetForecastAsync_WithNoMatchingLocation_ThrowsCityNotFoundException()
    {
        using var handler = new QueueHttpMessageHandler(["{ \"results\": [] }"]);
        using var httpClient = new HttpClient(handler);
        var client = new OpenMeteoClient(httpClient);

        await Assert.ThrowsExceptionAsync<CityNotFoundException>(
            () => client.GetForecastAsync("Ciudad inexistente", CancellationToken.None));
    }
}
