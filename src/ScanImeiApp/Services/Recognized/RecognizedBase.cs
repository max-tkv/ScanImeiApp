using Microsoft.Extensions.Logging;
using ScanImeiApp.Abstractions;
using ScanImeiApp.Lookups;
using ScanImeiApp.Options;
using ScanImeiApp.Tesseract.Abstractions;

namespace ScanImeiApp.Services.Recognized;

/// <summary>
/// Базовый класс представляет общий функционал распознавания с изображения.
/// </summary>
public class RecognizedBase
{
    protected readonly AppOptions _appOptions;
    private readonly ITesseractService _tesseractService;
    private readonly ILogger<RecognizedBase> _logger;
    private readonly IRegexService _regexService;

    protected RecognizedBase(
        AppOptions appOptions,
        ITesseractService tesseractService, 
        ILogger<RecognizedBase> logger,
        IRegexService regexService)
    {
        _appOptions = appOptions;
        _tesseractService = tesseractService;
        _logger= logger;
        _regexService = regexService;
    }

    /// <summary>
    /// Получить текст с изображения и получить IMEI из текста используя регулярные выражения.
    /// </summary>
    /// <param name="imageName">Имя изображения.</param>
    /// <param name="adjustStreamImage">Изображение.</param>
    /// <param name="recognizedImageType">Тип изменения изображения.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Список IMEI.</returns>
    protected async Task<List<string>> RecognizedAndExtractedImeiAsync(
        MemoryStream adjustStreamImage, 
        string imageName, 
        RecognizedImageType recognizedImageType,
        CancellationToken cancellationToken)
    {
        string recognizedText = RecognizedTextFromImage(adjustStreamImage);
        string formatRecognizedText = FormatRecognizeText(recognizedText);
        LogResultRecognized(imageName, formatRecognizedText, recognizedImageType);
        List<string> extractedImei = await _regexService.FindAndExtractedByPatternsAsync(
            formatRecognizedText, 
            _appOptions.Patterns,
            cancellationToken);
        LogResultExtractedImei(imageName, extractedImei, recognizedImageType);
        return extractedImei;
    }
    
    /// <summary>
    /// Получить текст с изображения.
    /// </summary>
    /// <param name="memoryStreamImage">Изображение.</param>
    /// <returns>Текст.</returns>
    private string RecognizedTextFromImage(MemoryStream memoryStreamImage) =>
        _tesseractService.Recognize(memoryStreamImage);
    
    /// <summary>
    /// Записать логи с результатами изъятия текста с изображения.
    /// </summary>
    /// <param name="imageName"></param>
    /// <param name="recognizedText"></param>
    /// <param name="recognizedImageType"></param>
    private void LogResultRecognized(
        string imageName, 
        string recognizedText, 
        RecognizedImageType recognizedImageType)
    {
        _logger.LogInformation($"Тип изменения: {recognizedImageType}. " +
                               $"Результат изъятия текста с изображения {imageName}:\n{recognizedText}");
    }
    
    /// <summary>
    /// Записать лог с результатами распознавания IMEI на изображении.
    /// </summary>
    /// <param name="imageName"></param>
    /// <param name="extractedImei"></param>
    /// <param name="recognizedImageType"></param>
    private void LogResultExtractedImei(
        string imageName, 
        List<string> extractedImei, 
        RecognizedImageType recognizedImageType)
    {
        _logger.LogInformation($"Тип изменения: {recognizedImageType}. " +
                               $"Результат распознавания IMEI на изображении {imageName}:\n" +
                               $"{string.Join(", ", extractedImei)}");
    }

    /// <summary>
    /// Форматировать распознанный текст с изображения.
    /// </summary>
    /// <param name="recognizeText">Распознанный текст.</param>
    /// <returns>Форматированный распознанный текст.</returns>
    private string FormatRecognizeText(string recognizeText)
    {
        string textWithoutSlash = _regexService.RemoveAfterSlash(recognizeText);
        return textWithoutSlash
            .Replace(" ", string.Empty)
            .Replace("\n", string.Empty)
            .Trim();
    }
}