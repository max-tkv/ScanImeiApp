using Microsoft.Extensions.Logging;
using ScanImeiApp.Abstractions;
using ScanImeiApp.Models;

namespace ScanImeiApp.Services;

/// <summary>
/// Класс представляет сервис по распознаванию текста на изображении.
/// </summary>
public class RecognizerTextService : IRecognizerTextService
{
    private readonly ITesseractService _tesseractService;
    private readonly ILogger<RecognizerTextService> _logger;

    public RecognizerTextService(
        ITesseractService tesseractService, 
        ILogger<RecognizerTextService> logger)
    {
        _tesseractService = tesseractService;
        _logger = logger;
    }
    
    /// <inheritdoc />
    public RecognizeResult RecognizeText(
        MemoryStream image,
        string imageName,
        string recognizerName)
    {
        RecognizeResult recognizedResult = RecognizedTextFromImage(image);
        recognizedResult.RecognizerName = recognizerName;
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
                         $"Имя распознавателя: {recognizedResult.RecognizerName}. " +
                         $"Уровень доверия к распознаванию OCR - {recognizedResult.Confidence}.\n" +
                         $"Результат изъятия текста с изображения:\n{recognizedResult.Text}");
    }

    #endregion
}