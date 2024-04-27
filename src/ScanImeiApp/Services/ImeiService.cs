using Microsoft.Extensions.Logging;
using ScanImeiApp.Abstractions;
using ScanImeiApp.Lookups;
using ScanImeiApp.Models;
using ScanImeiApp.Options;

namespace ScanImeiApp.Services;

/// <summary>
/// Класс для работы с IMEI.
/// </summary>
public class ImeiService : IImeiService
{
    private readonly ILogger<ImeiService> _logger;
    private readonly AppOptions _appOptions;
    private readonly IRegexService _regexService;

    public ImeiService(
        ILogger<ImeiService> logger, 
        AppOptions appOptions, 
        IRegexService regexService)
    {
        _logger = logger;
        _appOptions = appOptions;
        _regexService = regexService;
    }
    
    public async Task<List<string>> GetFromTextAsync(
        List<RecognizeResult> recognizeResults, 
        CancellationToken cancellationToken)
    {
        RecognizeResult bestRecognizeResult = recognizeResults
            .OrderByDescending(x => x.Confidence)
            .First();
        
        LogResultRecognized(
            bestRecognizeResult.ImageName, 
            bestRecognizeResult.Text, 
            bestRecognizeResult.RecognizeTextImageType,
            bestRecognizeResult.Confidence);
        
        string formatRecognizedText = FormatRecognizeText(bestRecognizeResult.Text);
        List<string> extractedImei = await _regexService.FindAndExtractedByPatternsAsync(
            formatRecognizedText, 
            _appOptions.Patterns,
            cancellationToken);
        
        LogResultExtractedImei(
            bestRecognizeResult.ImageName, 
            extractedImei);

        return extractedImei;
    }

    #region Приватные методы
    
    /// <summary>
    /// Записать логи с результатами изъятия текста с изображения.
    /// </summary>
    /// <param name="imageName">Имя изобоажения.</param>
    /// <param name="recognizedText">Распознанный текст.</param>
    /// <param name="recognizeTextImageType">Тип изменения изображения.</param>
    /// <param name="confidence">Уровень доверия к распознаванию OCR</param>
    private void LogResultRecognized(
        string imageName, 
        string recognizedText, 
        RecognizeTextImageType recognizeTextImageType,
        float confidence)
    {
        _logger.LogInformation($"Имя изображения: {imageName}\n" +
                               $"Лучший тип изменения: {recognizeTextImageType}. " +
                               $"Уровень доверия к распознаванию OCR - {confidence}.\n" +
                               $"Результат изъятия текста с изображения:\n{recognizedText}");
    }

    /// <summary>
    /// Записать лог с результатами распознавания IMEI на изображении.
    /// </summary>
    /// <param name="imageName">Имя изобоажения.</param>
    /// <param name="extractedImei">Распознанные IMEI.</param>
    private void LogResultExtractedImei(
        string imageName, 
        List<string> extractedImei)
    {
        _logger.LogInformation($"Имя изображения: {imageName}\n" +
                               $"Результат распознавания IMEI: " +
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

    #endregion
}