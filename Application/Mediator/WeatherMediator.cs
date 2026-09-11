using Microsoft.Extensions.DependencyInjection;

namespace weather_app.Application.Mediator;

public sealed class WeatherMediator(IServiceProvider serviceProvider) : IWeatherMediator
{
    public Task<TResponse> SendAsync<TQuery, TResponse>(TQuery query, CancellationToken cancellationToken = default)
        where TQuery : IQuery<TResponse>
    {
        var handler = serviceProvider.GetRequiredService<IQueryHandler<TQuery, TResponse>>();

        return handler.HandleAsync(query, cancellationToken);
    }
}
