using System.Globalization;
using Microsoft.AspNetCore.Components;
using weather_app.Application.Mediator;
using weather_app.Features.WeatherForecast.GetForecast;

namespace weather_app.Components.Pages;

public partial class Weather
{
    private static readonly CultureInfo SpanishCulture = CultureInfo.GetCultureInfo("es-MX");

    [Inject]
    private IWeatherMediator Mediator { get; set; } = null!;

    [Inject]
    private ILogger<Weather> Logger { get; set; } = null!;

    private WeatherSearchForm search = new();
    private WeatherForecastResponse? forecast;
    private string? errorMessage;
    private bool isLoading;
    private bool isFahrenheit;

    protected override Task OnInitializedAsync() => LoadForecastAsync();

    private Task SearchAsync() => LoadForecastAsync();

    private async Task LoadForecastAsync()
    {
        isLoading = true;
        errorMessage = null;

        try
        {
            forecast = await Mediator.SendAsync<GetWeatherForecastQuery, WeatherForecastResponse>(
                new GetWeatherForecastQuery(search.City),
                CancellationToken.None);
        }
        catch (CityNotFoundException)
        {
            Logger.LogWarning("No se encontró la ciudad solicitada: {City}", search.City);
            forecast = null;
            errorMessage = "No encontramos esa ciudad. Prueba con ciudad y país.";
        }
        catch (HttpRequestException exception)
        {
            Logger.LogError(exception, "El proveedor de clima no respondió para {City}", search.City);
            errorMessage = "El proveedor del clima no está disponible. Intenta nuevamente en unos minutos.";
        }
        catch (Exception exception)
        {
            Logger.LogError(exception, "No se pudo cargar el pronóstico para {City}", search.City);
            errorMessage = "No fue posible cargar el pronóstico. Intenta nuevamente.";
        }
        finally
        {
            isLoading = false;
        }
    }

    private static string FormatDate(DateOnly date) => date.ToString("dddd, d 'de' MMMM", SpanishCulture);

    private string FormatTemperature(double temperatureC)
    {
        var temperature = isFahrenheit ? temperatureC * 9 / 5 + 32 : temperatureC;
        var unit = isFahrenheit ? "°F" : "°C";

        return $"{Math.Round(temperature):0}{unit}";
    }

    private static string FormatMeasurement(double value) => value.ToString("0.#", SpanishCulture);
}
