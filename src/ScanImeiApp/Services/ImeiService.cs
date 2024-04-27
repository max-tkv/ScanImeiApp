using Microsoft.Extensions.Logging;
using ScanImeiApp.Abstractions;
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
        RecognizeResult bestRecognizeResult = GetBestRecognizeResult(recognizeResults);
        
        LogResultRecognized(
            bestRecognizeResult.ImageName, 
            bestRecognizeResult.Text, 
            bestRecognizeResult.RecognizerName,
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
    /// Получить лучший результат распознавания.
    /// </summary>
    /// <param name="recognizeResults">Результаты распознавания.</param>
    /// <returns>Лучший результат распознавания.</returns>
    private RecognizeResult GetBestRecognizeResult(List<RecognizeResult> recognizeResults)
    {
        IReadOnlyCollection<string> requiredTextImei = _appOptions.RequiredTextImei;
        var recognizeResultsWithRequiredTextImei = new List<RecognizeResult>();
        foreach (var recognizeResult in recognizeResults)
        {
            foreach (var requiredTextImeiItem in requiredTextImei)
            {
                if (recognizeResult.Text.ToLower().Contains(requiredTextImeiItem.ToLower()) && 
                    recognizeResultsWithRequiredTextImei.All(x => x.RecognizerName != recognizeResult.RecognizerName))
                {
                    recognizeResultsWithRequiredTextImei.Add(recognizeResult);
                }   
            }
        }
        
        if(!recognizeResultsWithRequiredTextImei.Any())
        {
            recognizeResultsWithRequiredTextImei = recognizeResults;
        }
        
        return recognizeResultsWithRequiredTextImei
            .Where(x => x.Text.Any())
            .OrderByDescending(x => x.Confidence)
            .First();
    }
    
    /// <summary>
    /// Записать логи с результатами изъятия текста с изображения.
    /// </summary>
    /// <param name="imageName">Имя изображения.</param>
    /// <param name="recognizedText">Распознанный текст.</param>
    /// <param name="recognizerName">Имя модификаций изображения.</param>
    /// <param name="confidence">Уровень доверия к распознаванию OCR</param>
    private void LogResultRecognized(
        string imageName, 
        string recognizedText, 
        string recognizerName,
        float confidence)
    {
        _logger.LogInformation($"Имя изображения: {imageName}\n" +
                               $"Лучший тип изменения: {recognizerName}. " +
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
            .Replace("\n", ":")
            .Replace("::", ":")
            .Trim();
    }

    #endregion
}