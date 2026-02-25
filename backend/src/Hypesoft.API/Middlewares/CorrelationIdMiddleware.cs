namespace Hypesoft.API.Middlewares;

public class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;
    private const string CorrelationIdHeader = "X-Correlation-ID";

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // pegao ID da request 
        if (!context.Request.Headers.TryGetValue(CorrelationIdHeader, out var correlationId))
        {
            correlationId = Guid.NewGuid().ToString();
        }

        // Adiciona ao contexto do Serilog
        using (Serilog.Context.LogContext.PushProperty("CorrelationId", correlationId))
        {
            context.Response.Headers.Append(CorrelationIdHeader, correlationId);
            await _next(context);
        }
    }
}