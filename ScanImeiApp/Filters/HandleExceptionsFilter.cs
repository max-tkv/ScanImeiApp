using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ScanImeiApp.Exceptions;

namespace ScanImeiApp.Filters;

/// <summary>
/// Фильтр обработки исключений.
/// </summary>
public class HandleExceptionsFilter : ExceptionFilterAttribute
{
    /// <inheritdoc />
    public override void OnException(ExceptionContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (context.Exception.GetType() == typeof(Exception))
        {
            return;
        }

        string errorMessage = GetErrorMessage(context.Exception);
        context.Result = new BadRequestObjectResult(errorMessage);
        context.ExceptionHandled = true;
        
        var logger = context.HttpContext.RequestServices
            .GetRequiredService<ILogger<HandleExceptionsFilter>>();
        logger.LogError(errorMessage);
    }

    private string GetErrorMessage(Exception contextException) =>
        contextException switch
        {
            NotFoundImeiException => "Не удалось найти IMEI. Пожалуйста настройте приложение.",
            NotFoundPatternsException => "Не удалось получить список паттернов из настроек приложения.",
            DllNotFoundException e => $"Не удалось загрузить Teseract. Ошибка: {e.Message}",
            Exception e => $"Ошибка при обработке изображения. Ошибка: {e.Message}",
            _ => $"Произошло необработанное исключение.",
        };
}