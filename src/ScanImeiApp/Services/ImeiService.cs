using Microsoft.Extensions.Logging;
using ScanImeiApp.Abstractions;
using ScanImeiApp.Extensions;
using ScanImeiApp.Models;
using ScanImeiApp.Options;

namespace ScanImeiApp.Services;

/// <summary>
/// Класс для работы с IMEI.
/// </summary>
public class ImeiService : IImeiService
{
    private const int ImeiLength = 15;
    private const int CountChecksFavorite = 3;
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
    
    /// <inheritdoc />
    public async Task<List<string>> FindImeiToRecognizeResultsAsync(
        List<RecognizeResult> recognizeResults, 
        CancellationToken cancellationToken)
    {
        List<RecognizeResult> favoriteRecognizeResult = GetFavoriteRecognizeResult(
            recognizeResults,
            CountChecksFavorite);
        return await ExecuteFindImeiAsync(
            favoriteRecognizeResult, 
            cancellationToken);
    }

    #region Приватные методы

    /// <summary>
    /// Выполнить поиск IMEI по списку результатов распознавания.
    /// </summary>
    /// <param name="recognizeResults">Результаты распознавании.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Список IMEI.</returns>
    private async Task<List<string>> ExecuteFindImeiAsync(
        List<RecognizeResult> recognizeResults, 
        CancellationToken cancellationToken)
    {
        for (int i = 0; i < recognizeResults.Count; i++)
        {
            RecognizeResult favoriteRecognizeResultItem = recognizeResults[i];
            LogResultRecognized(
                i + 1,
                favoriteRecognizeResultItem.ImageName, 
                favoriteRecognizeResultItem.Text, 
                favoriteRecognizeResultItem.RecognizerName,
                favoriteRecognizeResultItem.Confidence);
        
            string formatRecognizedText = FormatRecognizeText(favoriteRecognizeResultItem.Text);
            List<string> extractedImei = await _regexService.FindAndExtractedByPatternsAsync(
                formatRecognizedText, 
                _appOptions.Patterns,
                cancellationToken);

            if (extractedImei.Any())
            {
                LogResultExtractedImei(
                    favoriteRecognizeResultItem.ImageName, 
                    extractedImei);
                
                return extractedImei.Distinct().ToList();
            }
        }

        return new List<string>();
    }
    
    /// <summary>
    /// Получить 3 лучших результата распознавания.
    /// </summary>
    /// <param name="recognizeResults">Результаты распознавания.</param>
    /// <param name="favoriteCount">Количество лучших результатов.</param>
    /// <returns>Лучший результат распознавания.</returns>
    private List<RecognizeResult> GetFavoriteRecognizeResult(List<RecognizeResult> recognizeResults, int favoriteCount)
    {
        var recognizeResultsWithRequiredTextImei = FilterRecognizeResultsWithRequiredTextImei(recognizeResults);
        if(!recognizeResultsWithRequiredTextImei.Any())
        {
            recognizeResultsWithRequiredTextImei = recognizeResults;
        }
        
        return recognizeResultsWithRequiredTextImei
            .Where(x => x.Text.Any())
            .OrderByDescending(x => x.Confidence)
            .Take(favoriteCount)
            .ToList();
    }

    /// <summary>
    /// Отфильтровать результаты распознавания,
    /// оставив только с наличием обязательного слова из опций приложения.
    /// </summary>
    /// <param name="recognizeResults">Список результатов распознавания.</param>
    /// <returns>Отфильтрованный список результатов распознавания.</returns>
    private List<RecognizeResult> FilterRecognizeResultsWithRequiredTextImei(List<RecognizeResult> recognizeResults)
    {
        IReadOnlyCollection<string> requiredTextImei = _appOptions.RequiredTextImei;
        var recognizeResultsWithRequiredTextImei = new List<RecognizeResult>();
        foreach (var recognizeResult in recognizeResults)
        {
            foreach (var requiredTextImeiItem in requiredTextImei)
            {
                bool isContainsRequiredTextImeiAndNotExistToResult = IsContainsRequiredTextImeiAndNotExistToResult(
                    recognizeResult, 
                    requiredTextImeiItem,
                    recognizeResultsWithRequiredTextImei);
                if (isContainsRequiredTextImeiAndNotExistToResult)
                {
                    int lastIndexOf = recognizeResult.Text.ToLower().LastIndexOf(
                        requiredTextImeiItem.ToLower(),
                        StringComparison.Ordinal);
                    bool checkedCanFitImei = CheckCanFitImei(
                        recognizeResult.Text, 
                        lastIndexOf + requiredTextImeiItem.Length);
                    if (checkedCanFitImei)
                    {
                        recognizeResultsWithRequiredTextImei.Add(recognizeResult);   
                    }
                }   
            }
        }

        return recognizeResultsWithRequiredTextImei;
    }

    /// <summary>
    /// Содержит одно из обязательных слов и ранее не добавлен в результат.
    /// </summary>
    /// <param name="recognizeResult">Проверяемый результат распознавания.</param>
    /// <param name="requiredTextImeiItem">Обязательное слово для проверки.</param>
    /// <param name="recognizeResultsWithRequiredTextImei">Список с уже добавленными результатами распознавания.</param>
    /// <returns><b>True</b> - да, <b>False</b> - нет.</returns>
    private static bool IsContainsRequiredTextImeiAndNotExistToResult(
        RecognizeResult recognizeResult, 
        string requiredTextImeiItem, 
        List<RecognizeResult> recognizeResultsWithRequiredTextImei) =>
        recognizeResult.Text.ToLower().Contains(requiredTextImeiItem.ToLower()) && 
        recognizeResultsWithRequiredTextImei.All(x => x.RecognizerName != recognizeResult.RecognizerName);

    /// <summary>
    /// Проверить может ли вместиться IMEI.
    /// </summary>
    /// <param name="recognizeResultText">Текст для проверки.</param>
    /// <param name="lastIndexOf">С какого символа проверять.</param>
    /// <returns><b>True</b> - да, <b>False</b> - нет.</returns>
    private static bool CheckCanFitImei(string recognizeResultText, int lastIndexOf) =>
        recognizeResultText.Length - lastIndexOf >= ImeiLength;

    /// <summary>
    /// Записать логи с результатами изъятия текста с изображения.
    /// </summary>
    /// <param name="number">Номер.</param>
    /// <param name="imageName">Имя изображения.</param>
    /// <param name="recognizedText">Распознанный текст.</param>
    /// <param name="recognizerName">Имя модификаций изображения.</param>
    /// <param name="confidence">Уровень доверия к распознаванию OCR</param>
    private void LogResultRecognized(
        int number, 
        string imageName, 
        string recognizedText, 
        string recognizerName,
        float confidence)
    {
        var recognizedResultText = string.IsNullOrWhiteSpace(recognizedText) ? "-" : recognizedText;
        _logger.LogInformation($"Имя изображения: {imageName}\n" +
                               $"Лучший тип изменения №{number}: {recognizerName}.\n" +
                               $"Уровень доверия к распознаванию OCR - {confidence}.\n" +
                               $"Результат изъятия текста с изображения:\n{recognizedResultText}");
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
        string resultText = _regexService.RemoveAfterSlash(recognizeText);
        
        resultText = resultText
            .Replace(" ", string.Empty)
            .Replace("\n", ":")
            .Trim();

        resultText = resultText
            .ReplaceMultipleColons()
            .DeleteFirstColonsChar()
            .DeleteLastColonsChar();

        return resultText;
    }

    #endregion
}