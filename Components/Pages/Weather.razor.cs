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
}
