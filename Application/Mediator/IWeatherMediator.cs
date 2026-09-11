namespace weather_app.Application.Mediator;

public interface IWeatherMediator
{
    Task<TResponse> SendAsync<TQuery, TResponse>(TQuery query, CancellationToken cancellationToken = default)
        where TQuery : IQuery<TResponse>;
}
