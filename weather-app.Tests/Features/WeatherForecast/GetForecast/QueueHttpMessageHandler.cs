using System.Net;
using System.Text;

namespace weather_app.Tests.Features.WeatherForecast.GetForecast;

public sealed class QueueHttpMessageHandler(IEnumerable<string> responses) : HttpMessageHandler
{
    private readonly Queue<string> responseQueue = new(responses);
    private readonly List<Uri> requestUris = [];

    public IReadOnlyList<Uri> RequestUris => requestUris;

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        requestUris.Add(request.RequestUri ?? throw new InvalidOperationException("La solicitud no contiene una URI."));
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseQueue.Dequeue(), Encoding.UTF8, "application/json")
        };

        return Task.FromResult(response);
    }
}
