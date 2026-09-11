using weather_app.Components;
using weather_app.Application.Mediator;
using weather_app.Features.WeatherForecast.GetForecast;
using weather_app.Features.WeatherForecast.SearchCities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddMemoryCache();
builder.Services.AddScoped<IWeatherMediator, WeatherMediator>();
builder.Services.AddScoped<IQueryHandler<GetWeatherForecastQuery, WeatherForecastResponse>, GetWeatherForecastQueryHandler>();
builder.Services.AddScoped<IQueryHandler<SearchCitiesQuery, IReadOnlyList<CitySearchResult>>, SearchCitiesQueryHandler>();
builder.Services.AddHttpClient<IOpenMeteoClient, OpenMeteoClient>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(10);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
