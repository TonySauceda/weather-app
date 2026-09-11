using Microsoft.VisualStudio.TestTools.UnitTesting;
using weather_app.Features.WeatherForecast.GetForecast;
using weather_app.Features.WeatherForecast.SearchCities;

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

    [TestMethod]
    public async Task SearchCitiesAsync_WithProviderResults_MapsCandidatesAndUsesSearchParameters()
    {
        using var handler = new QueueHttpMessageHandler(
        [
            """
            {
              "results": [
                { "name": "Springfield", "admin1": "Illinois", "country": "Estados Unidos", "latitude": 39.798, "longitude": -89.644 },
                { "name": "Springfield", "admin1": "Missouri", "country": "Estados Unidos", "latitude": 37.208, "longitude": -93.292 },
                { "name": "Springfield", "admin1": "Massachusetts", "country": "Estados Unidos", "latitude": 42.102, "longitude": -72.589 },
                { "name": "Springfield", "admin1": "Oregon", "country": "Estados Unidos", "latitude": 44.046, "longitude": -123.022 },
                { "name": "Springfield", "admin1": "Ohio", "country": "Estados Unidos", "latitude": 39.924, "longitude": -83.808 },
                { "name": "Springfield", "admin1": "Tennessee", "country": "Estados Unidos", "latitude": 36.51, "longitude": -86.885 },
                { "name": "Springfield", "admin1": "Virginia", "country": "Estados Unidos", "latitude": 38.789, "longitude": -77.187 },
                { "name": "Springfield", "admin1": "Colorado", "country": "Estados Unidos", "latitude": 39.13, "longitude": -108.052 },
                { "name": "Springfield", "admin1": "Nueva Jersey", "country": "Estados Unidos", "latitude": 40.704, "longitude": -74.318 }
              ]
            }
            """
        ]);
        using var httpClient = new HttpClient(handler);
        var client = new OpenMeteoClient(httpClient);

        var results = await client.SearchCitiesAsync("Springfield", CancellationToken.None);

        Assert.AreEqual(8, results.Count);
        Assert.AreEqual(
            new CitySearchResult("Springfield", "Illinois", "Estados Unidos", 39.798, -89.644),
            results[0]);
        Assert.AreEqual("Springfield, Illinois, Estados Unidos", results[0].DisplayName);
        StringAssert.Contains(handler.RequestUris[0].Query, "count=8");
        StringAssert.Contains(handler.RequestUris[0].Query, "language=es");
    }

    [TestMethod]
    public async Task SearchCitiesAsync_WithNoProviderResults_ReturnsAnEmptyList()
    {
        using var handler = new QueueHttpMessageHandler(["{ \"results\": [] }"]);
        using var httpClient = new HttpClient(handler);
        var client = new OpenMeteoClient(httpClient);

        var results = await client.SearchCitiesAsync("Ciudad inexistente", CancellationToken.None);

        Assert.AreEqual(0, results.Count);
    }

    [TestMethod]
    public async Task GetForecastAsync_WithSelectedCity_UsesSelectedCoordinatesWithoutGeocodingAgain()
    {
        using var handler = new QueueHttpMessageHandler(
        [
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
        var selectedCity = new CitySearchResult("Springfield", "Illinois", "Estados Unidos", 39.798, -89.644);

        var forecast = await client.GetForecastAsync(selectedCity, CancellationToken.None);

        Assert.AreEqual("Springfield, Illinois, Estados Unidos", forecast.City);
        Assert.AreEqual(1, handler.RequestUris.Count);
        StringAssert.Contains(handler.RequestUris[0].Query, "latitude=39.798");
        StringAssert.Contains(handler.RequestUris[0].Query, "longitude=-89.644");
    }
}
