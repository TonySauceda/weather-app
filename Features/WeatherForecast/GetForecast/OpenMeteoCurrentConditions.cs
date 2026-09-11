using System.Text.Json.Serialization;

namespace weather_app.Features.WeatherForecast.GetForecast;

public sealed class OpenMeteoCurrentConditions
{
    [JsonPropertyName("time")]
    public DateTime Time { get; init; }

    [JsonPropertyName("temperature_2m")]
    public double TemperatureC { get; init; }

    [JsonPropertyName("apparent_temperature")]
    public double FeelsLikeC { get; init; }

    [JsonPropertyName("relative_humidity_2m")]
    public int HumidityPercentage { get; init; }

    [JsonPropertyName("wind_speed_10m")]
    public double WindSpeedKph { get; init; }

    [JsonPropertyName("weather_code")]
    public int WeatherCode { get; init; }
}
