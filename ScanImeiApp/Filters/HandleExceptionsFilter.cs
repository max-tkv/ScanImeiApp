using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ScanImeiApp.Exceptions;

namespace ScanImeiApp.Filters;

/// <summary>
/// Фильтр обработки исключений.
/// </summary>
public class HandleExceptionsFilter : Attribute, IAsyncActionFilter
{
    private readonly ILogger<HandleExceptionsFilter> _logger;

    /// <summary>
    /// Конструктор фильтра обработки исключений.
    /// </summary>
    /// <param name="logger"></param>
    public HandleExceptionsFilter(ILogger<HandleExceptionsFilter> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        try
        {
            await next();
        }
        catch (NotFoundImeiException notFoundImeiException)
        {
            var errorMessage = "Не удалось найти IMEI.";
            _logger.LogWarning(notFoundImeiException, errorMessage);
            context.Result = new BadRequestObjectResult(errorMessage);
        }
        catch (NotFoundPatternsException notFoundPatterns)
        {
            var errorMessage = "Не удалось получить список паттернов из настроек приложения.";
            _logger.LogWarning(notFoundPatterns, errorMessage);
            context.Result = new BadRequestObjectResult(errorMessage);
        }
        catch (DllNotFoundException dllNotFoundException)
        {
            var errorMessage = $"Не удалось загрузить Teseract. Ошибка: {dllNotFoundException.Message}";
            _logger.LogWarning(dllNotFoundException, errorMessage);
            context.Result = new BadRequestObjectResult(errorMessage);
        }
        catch (Exception ex)
        {
            var errorMessage = "Ошибка при обработке изображения.";
            _logger.LogError(ex, errorMessage);
            context.Result = new BadRequestObjectResult(errorMessage);
        }
    }
}