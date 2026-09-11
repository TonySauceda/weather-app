using System.Text.Json.Serialization;

namespace weather_app.Features.WeatherForecast.GetForecast;

public sealed class OpenMeteoDailyForecast
{
    [JsonPropertyName("time")]
    public List<DateOnly>? Dates { get; init; }

    [JsonPropertyName("weather_code")]
    public List<int>? WeatherCodes { get; init; }

    [JsonPropertyName("temperature_2m_min")]
    public List<double>? MinimumTemperaturesC { get; init; }

    [JsonPropertyName("temperature_2m_max")]
    public List<double>? MaximumTemperaturesC { get; init; }

    [JsonPropertyName("precipitation_probability_max")]
    public List<int>? PrecipitationProbabilities { get; init; }

    [JsonPropertyName("precipitation_sum")]
    public List<double>? PrecipitationMillimeters { get; init; }
}
