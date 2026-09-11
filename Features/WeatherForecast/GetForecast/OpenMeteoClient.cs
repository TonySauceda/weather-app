using System.Globalization;
using System.Net.Http.Json;

namespace weather_app.Features.WeatherForecast.GetForecast;

public sealed class OpenMeteoClient(HttpClient httpClient) : IOpenMeteoClient
{
    public async Task<WeatherForecastResponse> GetForecastAsync(string city, CancellationToken cancellationToken)
    {
        var location = await GetLocationAsync(city, cancellationToken);
        var forecastResponse = await httpClient.GetFromJsonAsync<OpenMeteoForecastApiResponse>(
            CreateForecastUrl(location),
            cancellationToken);

        if (forecastResponse?.Current is not { } current || forecastResponse.Daily is not { } daily)
        {
            throw new HttpRequestException("Open-Meteo devolvió una respuesta de pronóstico incompleta.");
        }

        var upcomingDays = CreateDailyForecasts(daily, DateOnly.FromDateTime(current.Time));
        if (upcomingDays.Length != 5)
        {
            throw new HttpRequestException("Open-Meteo no devolvió cinco días de pronóstico.");
        }

        return new WeatherForecastResponse(
            FormatLocation(location),
            new CurrentWeatherResponse(
                DateOnly.FromDateTime(current.Time),
                current.TemperatureC,
                current.FeelsLikeC,
                current.HumidityPercentage,
                current.WindSpeedKph,
                GetSummary(current.WeatherCode)),
            upcomingDays);
    }

    private async Task<OpenMeteoLocation> GetLocationAsync(string city, CancellationToken cancellationToken)
    {
        var response = await httpClient.GetFromJsonAsync<OpenMeteoGeocodingResponse>(
            $"https://geocoding-api.open-meteo.com/v1/search?name={Uri.EscapeDataString(city)}&count=1&language=es&format=json",
            cancellationToken);

        return response?.Results?.FirstOrDefault() ?? throw new CityNotFoundException(city);
    }

    private static string CreateForecastUrl(OpenMeteoLocation location)
    {
        var latitude = location.Latitude.ToString(CultureInfo.InvariantCulture);
        var longitude = location.Longitude.ToString(CultureInfo.InvariantCulture);

        return $"https://api.open-meteo.com/v1/forecast?latitude={latitude}&longitude={longitude}" +
            "&current=temperature_2m,apparent_temperature,relative_humidity_2m,wind_speed_10m,weather_code" +
            "&daily=weather_code,temperature_2m_min,temperature_2m_max,precipitation_probability_max,precipitation_sum" +
            "&forecast_days=6&timezone=auto";
    }

    private static DailyForecastResponse[] CreateDailyForecasts(OpenMeteoDailyForecast daily, DateOnly currentDate)
    {
        if (daily.Dates is null || daily.WeatherCodes is null || daily.MinimumTemperaturesC is null ||
            daily.MaximumTemperaturesC is null || daily.PrecipitationProbabilities is null ||
            daily.PrecipitationMillimeters is null)
        {
            throw new HttpRequestException("Open-Meteo devolvió datos diarios incompletos.");
        }

        var numberOfDays = daily.Dates.Count;
        if (daily.WeatherCodes.Count != numberOfDays || daily.MinimumTemperaturesC.Count != numberOfDays ||
            daily.MaximumTemperaturesC.Count != numberOfDays || daily.PrecipitationProbabilities.Count != numberOfDays ||
            daily.PrecipitationMillimeters.Count != numberOfDays)
        {
            throw new HttpRequestException("Open-Meteo devolvió series diarias inconsistentes.");
        }

        return Enumerable.Range(0, numberOfDays)
            .Select(index => new DailyForecastResponse(
                daily.Dates[index],
                daily.MinimumTemperaturesC[index],
                daily.MaximumTemperaturesC[index],
                GetSummary(daily.WeatherCodes[index]),
                daily.PrecipitationProbabilities[index],
                daily.PrecipitationMillimeters[index]))
            .Where(forecast => forecast.Date > currentDate)
            .Take(5)
            .ToArray();
    }

    private static string FormatLocation(OpenMeteoLocation location)
    {
        return string.Join(
            ", ",
            new[] { location.Name, location.AdministrativeArea, location.Country }
                .Where(value => !string.IsNullOrWhiteSpace(value)));
    }

    private static string GetSummary(int weatherCode) => weatherCode switch
    {
        0 => "Soleado",
        1 or 2 => "Parcialmente nublado",
        3 or 45 or 48 => "Nublado",
        >= 51 and <= 99 => "Lluvioso",
        _ => "Nublado"
    };
}
