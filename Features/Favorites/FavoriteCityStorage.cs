using Microsoft.JSInterop;

namespace weather_app.Features.Favorites;

public sealed class FavoriteCityStorage(IJSRuntime js) : IAsyncDisposable
{
    private const string ModulePath = "./Components/Pages/Weather.razor.js";
    private const string LoadMethod = "loadFavorites";
    private const string SaveMethod = "saveFavorites";

    private IJSObjectReference? module;

    public async Task<IReadOnlyList<FavoriteCity>> LoadAsync()
    {
        var storageModule = await GetModuleAsync();
        return await storageModule.InvokeAsync<FavoriteCity[]>(LoadMethod) ?? [];
    }

    public async Task SaveAsync(IReadOnlyList<FavoriteCity> favorites)
    {
        var storageModule = await GetModuleAsync();
        await storageModule.InvokeVoidAsync(SaveMethod, favorites);
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            if (module is not null)
            {
                await module.DisposeAsync();
            }
        }
        catch (JSDisconnectedException)
        {
        }
    }

    private async ValueTask<IJSObjectReference> GetModuleAsync() =>
        module ??= await js.InvokeAsync<IJSObjectReference>("import", ModulePath);
}
