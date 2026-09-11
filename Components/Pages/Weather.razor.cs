using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using weather_app.Application.Mediator;
using weather_app.Features.WeatherForecast.GetForecast;
using weather_app.Features.WeatherForecast.SearchCities;

namespace weather_app.Components.Pages;

public partial class Weather : IAsyncDisposable
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
    private IReadOnlyList<CitySearchResult> citySuggestions = [];
    private CitySearchResult? selectedCity;
    private CancellationTokenSource? citySearchCancellationTokenSource;
    private int activeCitySuggestionIndex = -1;
    private bool hasSearchedCities;

    private bool HasActiveCitySuggestion =>
        activeCitySuggestionIndex >= 0 && activeCitySuggestionIndex < citySuggestions.Count;

    protected override Task OnInitializedAsync() => LoadForecastAsync();

    private Task SearchAsync()
    {
        ClearCitySuggestions();

        return LoadForecastAsync(selectedCity);
    }

    private async Task OnCityInputAsync(ChangeEventArgs eventArgs)
    {
        search.City = eventArgs.Value?.ToString() ?? string.Empty;
        selectedCity = null;
        ClearCitySuggestions();

        var searchText = search.City.Trim();
        if (searchText.Length < 2)
        {
            return;
        }

        using var cancellationTokenSource = new CancellationTokenSource();
        citySearchCancellationTokenSource = cancellationTokenSource;

        try
        {
            await Task.Delay(TimeSpan.FromMilliseconds(300), cancellationTokenSource.Token);
            var suggestions = await Mediator.SendAsync<SearchCitiesQuery, IReadOnlyList<CitySearchResult>>(
                new SearchCitiesQuery(searchText),
                cancellationTokenSource.Token);

            if (citySearchCancellationTokenSource != cancellationTokenSource)
            {
                return;
            }

            citySuggestions = suggestions;
            hasSearchedCities = true;
        }
        catch (OperationCanceledException) when (cancellationTokenSource.IsCancellationRequested)
        {
        }
        catch (HttpRequestException exception)
        {
            Logger.LogWarning(exception, "No se pudieron obtener sugerencias para {SearchText}", searchText);
        }
        catch (Exception exception)
        {
            Logger.LogError(exception, "No se pudieron obtener sugerencias para {SearchText}", searchText);
        }
        finally
        {
            if (citySearchCancellationTokenSource == cancellationTokenSource)
            {
                citySearchCancellationTokenSource = null;
            }
        }
    }

    private async Task HandleSuggestionKeyDownAsync(KeyboardEventArgs eventArgs)
    {
        if (citySuggestions.Count == 0)
        {
            return;
        }

        switch (eventArgs.Key)
        {
            case "ArrowDown":
                activeCitySuggestionIndex = Math.Min(activeCitySuggestionIndex + 1, citySuggestions.Count - 1);
                break;
            case "ArrowUp":
                activeCitySuggestionIndex = activeCitySuggestionIndex <= 0
                    ? citySuggestions.Count - 1
                    : activeCitySuggestionIndex - 1;
                break;
            case "Enter" when HasActiveCitySuggestion:
                await SelectCityAsync(citySuggestions[activeCitySuggestionIndex]);
                break;
            case "Escape":
                ClearCitySuggestions();
                break;
        }
    }

    private async Task SelectCityAsync(CitySearchResult city)
    {
        selectedCity = city;
        search.City = city.DisplayName;
        ClearCitySuggestions();

        await LoadForecastAsync(city);
    }

    private void ClearCitySuggestions()
    {
        citySearchCancellationTokenSource?.Cancel();
        citySearchCancellationTokenSource = null;
        citySuggestions = [];
        activeCitySuggestionIndex = -1;
        hasSearchedCities = false;
    }

    private async Task LoadForecastAsync(CitySearchResult? city = null)
    {
        isLoading = true;
        errorMessage = null;

        try
        {
            forecast = await Mediator.SendAsync<GetWeatherForecastQuery, WeatherForecastResponse>(
                new GetWeatherForecastQuery(search.City, city),
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

    public ValueTask DisposeAsync()
    {
        citySearchCancellationTokenSource?.Cancel();
        citySearchCancellationTokenSource?.Dispose();

        return ValueTask.CompletedTask;
    }
}
