using System.ComponentModel.DataAnnotations;

namespace weather_app.Components.Pages;

public sealed class WeatherSearchForm
{
    [Required(ErrorMessage = "Ingresa una ciudad.")]
    [StringLength(100, ErrorMessage = "La ciudad no puede exceder 100 caracteres.")]
    public string City { get; set; } = "Mazatlán";
}
