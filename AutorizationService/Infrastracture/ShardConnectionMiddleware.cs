namespace KafOCars.Infrastracture;

public class ShardConnectionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IShardConnectionResolver _shardConnectionResolver;

    public ShardConnectionMiddleware(RequestDelegate next, IShardConnectionResolver shardConnectionResolver)
    {
        _next = next;
        _shardConnectionResolver = shardConnectionResolver;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var shardKey = context.Request.Headers["Shard-Key"].FirstOrDefault();
        if (string.IsNullOrEmpty(shardKey))
        {
            context.Response.StatusCode = 400; // Bad Request
            await context.Response.WriteAsync("Shard-Key header is missing.");
            return;
        }

        var connectionString = _shardConnectionResolver.ResolveConnectionString(shardKey);
        if (connectionString == null)
        {
            context.Response.StatusCode = 404; // Not Found
            await context.Response.WriteAsync("Shard not found.");
            return;
        }

        context.Items["ShardConnectionString"] = connectionString;
        await _next(context);
    }
}
