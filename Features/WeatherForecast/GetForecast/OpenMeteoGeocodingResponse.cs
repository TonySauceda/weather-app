using System.Text.Json.Serialization;

namespace weather_app.Features.WeatherForecast.GetForecast;

public sealed class OpenMeteoGeocodingResponse
{
    [JsonPropertyName("results")]
    public List<OpenMeteoLocation>? Results { get; init; }
}
