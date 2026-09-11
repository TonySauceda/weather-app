using System.Text.Json.Serialization;

namespace weather_app.Features.WeatherForecast.GetForecast;

public sealed class OpenMeteoForecastApiResponse
{
    [JsonPropertyName("current")]
    public OpenMeteoCurrentConditions? Current { get; init; }

    [JsonPropertyName("daily")]
    public OpenMeteoDailyForecast? Daily { get; init; }
}
