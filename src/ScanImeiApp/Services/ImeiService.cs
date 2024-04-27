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
        // if (_appOptions.EnabledSimilarImei)
        // {
        //     RemoveImeiSimilar(recognizeResult, resultAll);
        // }
        // else
        // {
        //     resultAll.AddRange(recognizeResult);
        // }
        
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
    /// Удалить похожие IMEI.
    /// </summary>
    /// <remarks>
    /// IMEI (International Mobile Equipment Identity) — это уникальный идентификатор мобильного устройства.
    /// Каждый IMEI состоит из 15 цифр. В случае наличия двух IMEI на одном телефоне
    /// (например, у некоторых моделей смартфонов с поддержкой двух SIM-карт),
    /// они могут отличаться только в ПОСЛЕДНИХ 3 цифрах.
    /// </remarks>
    /// <param name="recognizeImeiList">IMEI для добаления.</param>
    /// <param name="resultAll">Уже добавленные IMEI.</param>
    /// <returns>True - похожи. False - не похожи.</returns>
    private void RemoveImeiSimilar(IEnumerable<string> recognizeImeiList, List<string> resultAll)
    {
        foreach (var recognizeImei in recognizeImeiList)
        {
            if (!resultAll.Any())
            {
                resultAll.Add(recognizeImei);
                continue;
            }
                    
            foreach (var resultItem in resultAll.ToList())
            {
                if (AreImeiSimilar(recognizeImei, resultItem) &&
                    resultAll.All(x => x != recognizeImei))
                {
                    resultAll.Add(recognizeImei);
                }
            }
        }
    }
    
    /// <summary>
    /// Определить похоже ли IMEI. Отличаются последние 6 символов.
    /// </summary>
    /// <param name="imei1">IMEI 1.</param>
    /// <param name="imei2">IMEI 2.</param>
    /// <returns>True - похожи. False - не похожи.</returns>
    private bool AreImeiSimilar(string imei1, string imei2)
    {
        if (imei1 == imei2)
        {
            return false;
        }
        
        if (EqualFirstPartImei(imei1, imei2, 9) & !EqualLastPartImei(imei1, imei2, 6))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Проверить первую часть IMEI (10 цыфр).
    /// </summary>
    /// <param name="imei1">IMEI 1.</param>
    /// <param name="imei2">IMEI 2.</param>
    /// <param name="numberCount">Количество последних сиволов.</param>
    /// <returns>True - равны. False - не равны.</returns>
    private static bool EqualLastPartImei(string imei1, string imei2, int numberCount)
    {
        int length = imei1.Length;
        if (imei1[(length - numberCount)..] != imei2[(length - numberCount)..])
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Прверить последню часть IMEI (5 цыфр).
    /// </summary>
    /// <param name="imei1">IMEI 1.</param>
    /// <param name="imei2">IMEI 2.</param>
    /// <param name="numberCount">Количество первых символов.</param>
    /// <returns>True - равны. False - не равны.</returns>
    private static bool EqualFirstPartImei(string imei1, string imei2, int numberCount)
    {
        for (int i = 0; i < numberCount; i++)
        {
            if (imei1[i] != imei2[i])
            {
                return false;
            }
        }

        return true;
    }
    
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