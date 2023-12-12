using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;

namespace RestaurantAPI.Middleware;

public class RequestTimeMiddleware : IMiddleware
{
    private readonly Stopwatch _stopwatch;
    private readonly ILogger<RequestTimeMiddleware> _logger;

    public RequestTimeMiddleware(ILogger<RequestTimeMiddleware> logger)
    {
        _stopwatch = new Stopwatch();
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        _stopwatch.Start();
        await next.Invoke(context);
        _stopwatch.Stop();

        var ellapsedMilliseconds = _stopwatch.ElapsedMilliseconds;
        if(ellapsedMilliseconds / 1000 > 4)
        {
            var message = $"Request [{context.Request.Method}] at {context.Request.Path} took {ellapsedMilliseconds} ms";
            _logger.LogInformation(message);
        }
    }
}
