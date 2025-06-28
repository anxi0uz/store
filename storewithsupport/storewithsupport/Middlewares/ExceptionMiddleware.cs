using Microsoft.EntityFrameworkCore;

namespace storewithsupport.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(
        RequestDelegate next,
        ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Необработанное исключение: {Message}", ex.Message);

            var statusCode = StatusCodes.Status500InternalServerError;
            var message = "Внутренняя ошибка сервера.";

            switch (ex)
            {
                case ArgumentException _:
                    statusCode = StatusCodes.Status400BadRequest;
                    message = "Ошибка валидации.";
                    break;
                case KeyNotFoundException _:
                    statusCode = StatusCodes.Status404NotFound;
                    message = "Ресурс не найден.";
                    break;
                case DbUpdateException _:
                    statusCode = StatusCodes.Status409Conflict;
                    message = "Ошибка в базе данных";
                    break;
            }
            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsJsonAsync(message);
        }
    }
}