using weather_app.Application.Mediator;

namespace weather_app.Features.WeatherForecast.GetForecast;

public sealed class GetWeatherForecastQueryHandler : IQueryHandler<GetWeatherForecastQuery, WeatherForecastResponse>
{
    private static readonly string[] Summaries = ["Soleado", "Parcialmente nublado", "Nublado", "Lluvioso", "Con brisa"];

    public async Task<WeatherForecastResponse> HandleAsync(
        GetWeatherForecastQuery query,
        CancellationToken cancellationToken)
    {
        var city = query.City.Trim();
        if (string.IsNullOrWhiteSpace(city))
        {
            throw new ArgumentException("La ciudad es obligatoria.", nameof(query));
        }

        await Task.Delay(TimeSpan.FromMilliseconds(350), cancellationToken);

        var random = new Random(StringComparer.OrdinalIgnoreCase.GetHashCode(city));
        var currentTemperature = random.Next(8, 34);
        var currentDate = DateOnly.FromDateTime(DateTime.Today);
        var forecasts = Enumerable.Range(1, 5)
            .Select(dayOffset => CreateDailyForecast(currentDate.AddDays(dayOffset), random))
            .ToArray();

        return new WeatherForecastResponse(
            city,
            new CurrentWeatherResponse(
                currentDate,
                currentTemperature,
                currentTemperature + random.Next(-2, 4),
                GetSummary(random)),
            forecasts);
    }

    private static DailyForecastResponse CreateDailyForecast(DateOnly date, Random random)
    {
        var minimumTemperature = random.Next(4, 20);
        var maximumTemperature = random.Next(minimumTemperature + 3, 37);

        return new DailyForecastResponse(date, minimumTemperature, maximumTemperature, GetSummary(random));
    }

    private static string GetSummary(Random random) => Summaries[random.Next(Summaries.Length)];
}
