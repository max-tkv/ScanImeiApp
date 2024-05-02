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

        string errorMessage = GetErrorMessageByException(context.Exception);
        var errorResponse = new ErrorResponse(errorMessage);
        context.Result = new BadRequestObjectResult(errorResponse);
        context.ExceptionHandled = true;
        
        ErrorLogMessage(context, errorMessage);
    }

    #region Приватные методы

    /// <summary>
    /// Записать лог с ошибкой.
    /// </summary>
    /// <param name="context">Контекст исключения.</param>
    /// <param name="errorMessage">Сообщение об ошибки.</param>
    private static void ErrorLogMessage(ExceptionContext context, string errorMessage)
    {
        var logger = context.HttpContext.RequestServices
            .GetRequiredService<ILogger<HandleExceptionsFilter>>();
        logger.LogError(errorMessage);
    }

    /// <summary>
    /// Получить текст ошибки по типу исключения.
    /// </summary>
    /// <param name="contextException">Контекст исключения.</param>
    /// <returns>Текст ошибки.</returns>
    private static string GetErrorMessageByException(Exception contextException) =>
        contextException switch
        {
            UnknownImageFormatException => "Не поддерживаемый формат изображения.",
            NotFoundImeiException => "Не удалось найти IMEI. Пожалуйста настройте приложение.",
            NotFoundAppOptionsException => "Не удалось получить настройки приложения.",
            DllNotFoundException e => $"Не удалось загрузить Tesseract. Ошибка: {e.GetExceptionMessage()}",
            _ => $"Произошло необработанное исключение. Описание: {contextException.GetExceptionMessage()}",
        };

    #endregion
}