using System.Text.RegularExpressions;
using ScanImeiApp.Abstractions;
using ScanImeiApp.Exceptions;
using ScanImeiApp.Tesseract.Abstractions;
using Tesseract;

namespace ScanImeiApp.Services;

/// <summary>
/// Класс представляющий возможность сканирования изображения на наличие IMEI при помощи OCR Teseract.
/// </summary>
public class ScanImeiTextService : IScanImeiTextService
{
    private const string PatternsConfigurationKey = "Patterns";
    private readonly List<string> _imeiPatterns;
    private readonly IImageService _imageService;
    private readonly ILogger<ScanImeiTextService> _logger;
    private readonly ITesseractService _tesseractService;

    public ScanImeiTextService(
        IConfiguration configuration, 
        IImageService imageService, 
        ITesseractService tesseractService,
        ILogger<ScanImeiTextService> logger)
    {
        _imeiPatterns = configuration
            .GetRequiredSection(PatternsConfigurationKey)
            .Get<List<string>>() ?? throw new NotFoundPatternsException();
        _imageService = imageService;
        _tesseractService = tesseractService;
        _logger = logger;
    }

    /// <inheritdoc />
    public List<string> GetImeiTextFromImage(MemoryStream memoryStreamImage, string imageName)
    {
        var result = new List<string>();
        
        List<string> extractedImei = RecognizedAndExtractedImei(memoryStreamImage, imageName);
        result.AddRange(extractedImei);
        
        MemoryStream adjustStreamImage = _imageService.AdjustContrast(memoryStreamImage, 50);
        List<string> extractedImeiAfterAdjustContrast = RecognizedAndExtractedImei(adjustStreamImage, imageName);
        result.AddRange(extractedImeiAfterAdjustContrast);
        
        return result.Distinct().ToList();
    }

    #region Приватные методы

    /// <summary>
    /// Получить текст с изображения и получить IMEI из текста используя регулярные выражения.
    /// </summary>
    /// <param name="imageName">Имя изображения.</param>
    /// <param name="adjustStreamImage">Изображение.</param>
    /// <returns>Список IMEI.</returns>
    private List<string> RecognizedAndExtractedImei(MemoryStream adjustStreamImage, string imageName)
    {
        string recognizedText = RecognizedTextFromImage(adjustStreamImage);
        LogResultRecognized(imageName, recognizedText);
        List<string> extractedImei = ExtractImeiFromText(recognizedText);
        LogResultExtractedImei(imageName, extractedImei);
        return extractedImei;
    }
    
    /// <summary>
    /// Получить текст с изображения.
    /// </summary>
    /// <param name="memoryStreamImage">Изображение.</param>
    /// <returns>Текст.</returns>
    private string RecognizedTextFromImage(MemoryStream memoryStreamImage) =>
        _tesseractService.Recognize(memoryStreamImage, true);

    /// <summary>
    /// Получить IMEI из текста используя регулярные выражения.
    /// </summary>
    /// <param name="recognizedText">Текст.</param>
    /// <returns>Список найденных IMEI</returns>
    /// <exception cref="NotFoundImeiException">Не удалось найти IMEI.</exception>
    private List<string> ExtractImeiFromText(string recognizedText)
    {
        var result = new List<string>();
        foreach (var pattern in _imeiPatterns)
        {
            Regex regex = new Regex(pattern);
            MatchCollection matches = regex.Matches(recognizedText);
            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    result.Add(match.Groups[1].Value);
                }
            }
        }

        return result;
    }
    
    private void LogResultRecognized(string imageName, string recognizedText)
    {
        _logger.LogInformation($"Результат изъятия текста с изображения {imageName}:\n{recognizedText}");
    }
    
    private void LogResultExtractedImei(string imageName, List<string> extractedImei)
    {
        _logger.LogInformation($"Результат распознавания IMEI на изображении {imageName}:\n" +
                               $"{string.Join(", ", extractedImei)}");
    }

    #endregion
}