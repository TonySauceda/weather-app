using System.Text.Json.Serialization;

namespace weather_app.Features.WeatherForecast.GetForecast;

public sealed class OpenMeteoLocation
{
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("admin1")]
    public string? AdministrativeArea { get; init; }

    [JsonPropertyName("country")]
    public string? Country { get; init; }

    [JsonPropertyName("latitude")]
    public double Latitude { get; init; }

    [JsonPropertyName("longitude")]
    public double Longitude { get; init; }
}
