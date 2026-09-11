using Microsoft.AspNetCore.Components;

namespace weather_app.Components.WeatherVisuals;

public partial class WeatherIcon
{
    [Parameter, EditorRequired]
    public string Summary { get; set; } = string.Empty;
}
