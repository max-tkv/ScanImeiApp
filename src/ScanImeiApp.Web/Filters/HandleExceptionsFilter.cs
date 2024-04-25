using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ScanImeiApp.Exceptions;
using ScanImeiApp.Contracts.Models;
using ScanImeiApp.Extensions;
using SixLabors.ImageSharp;

namespace ScanImeiApp.Web.Filters;

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
        var errorResponse = new ErrorResponse(errorMessage);
        context.Result = new BadRequestObjectResult(errorResponse);
        context.ExceptionHandled = true;
        
        ErrorLogMessage(context, errorMessage);
    }

    private static void ErrorLogMessage(ExceptionContext context, string errorMessage)
    {
        var logger = context.HttpContext.RequestServices
            .GetRequiredService<ILogger<HandleExceptionsFilter>>();
        logger.LogError(errorMessage);
    }

    private static string GetErrorMessage(Exception contextException) =>
        contextException switch
        {
            UnknownImageFormatException => "Не поддерживаемый формат изображения.",
            NotFoundImeiException => "Не удалось найти IMEI. Пожалуйста настройте приложение.",
            NotFoundAppOptionsException => "Не удалось получить настройки приложения.",
            DllNotFoundException e => $"Не удалось загрузить Teseract. Ошибка: {e.GetExceptionMessage()}",
            Exception e => $"Ошибка при обработке изображения. Ошибка: {e.GetExceptionMessage()}",
            _ => "Произошло необработанное исключение.",
        };
}