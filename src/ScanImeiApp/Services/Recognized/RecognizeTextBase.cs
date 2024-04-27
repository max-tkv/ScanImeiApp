using Microsoft.Extensions.Logging;
using ScanImeiApp.Abstractions;
using ScanImeiApp.Lookups;
using ScanImeiApp.Models;
using ScanImeiApp.Options;

namespace ScanImeiApp.Services.Recognized;

/// <summary>
/// Базовый класс представляет общий функционал распознавания с изображения.
/// </summary>
public class RecognizeTextBase
{
    protected readonly AppOptions _appOptions;
    private readonly ITesseractService _tesseractService;
    private readonly ILogger<RecognizeTextBase> _logger;

    protected RecognizeTextBase(
        AppOptions appOptions,
        ITesseractService tesseractService, 
        ILogger<RecognizeTextBase> logger)
    {
        _appOptions = appOptions;
        _tesseractService = tesseractService;
        _logger= logger;
    }

    /// <summary>
    /// Получить текст с изображения и получить IMEI из текста используя регулярные выражения.
    /// </summary>
    /// <param name="imageName">Имя изображения.</param>
    /// <param name="adjustStreamImage">Изображение.</param>
    /// <param name="recognizeTextImageType">Тип изменения изображения.</param>
    /// <returns>Список IMEI.</returns>
    protected RecognizeResult RecognizeText(
        MemoryStream adjustStreamImage, 
        string imageName, 
        RecognizeTextImageType recognizeTextImageType)
    {
        RecognizeResult recognizedResult = RecognizedTextFromImage(adjustStreamImage);
        recognizedResult.RecognizeTextImageType = recognizeTextImageType;
        recognizedResult.ImageName = imageName;

        LogDebugResultRecognizedText(recognizedResult);
        
        return recognizedResult;
    }

    #region Приватные методы

    /// <summary>
    /// Получить текст с изображения.
    /// </summary>
    /// <param name="memoryStreamImage">Изображение.</param>
    /// <returns>Текст.</returns>
    private RecognizeResult RecognizedTextFromImage(MemoryStream memoryStreamImage) =>
        _tesseractService.Recognize(memoryStreamImage);
    
    /// <summary>
    /// Записать лог для отладки с данными распознавания.
    /// </summary>
    /// <param name="recognizedResult">Данными распознавания.</param>
    private void LogDebugResultRecognizedText(RecognizeResult recognizedResult)
    {
        _logger.LogDebug($"Имя изображения: {recognizedResult.ImageName}\n" +
                         $"Тип изменения: {recognizedResult.RecognizeTextImageType}. " +
                         $"Уровень доверия к распознаванию OCR - {recognizedResult.Confidence}.\n" +
                         $"Результат изъятия текста с изображения:\n{recognizedResult.Text}");
    }

    #endregion
}