using System.Text.RegularExpressions;
using ScanImeiApp.Abstractions;
using ScanImeiApp.Exceptions;
using Tesseract;

namespace ScanImeiApp.Services;

/// <summary>
/// Класс представляющий возможность сканирования изображения на наличие IMEI при помощи OCR Teseract.
/// </summary>
public class ScanImeiTextService : IScanImeiTextService
{
    private const string TesseractLanguageName = "eng";
    private const string TesseractTessdataPath = "tessdata";
    private const string PatternsConfigurationKey = "Patterns";
    private readonly List<string> _imeiPatterns;
    private readonly IImageService _imageService;
    private readonly ILogger<ScanImeiTextService> _logger;

    /// <summary>
    /// .ctor
    /// </summary>
    public ScanImeiTextService(
        IConfiguration configuration, 
        IImageService imageService, 
        ILogger<ScanImeiTextService> logger)
    {
        _imeiPatterns = configuration
            .GetRequiredSection(PatternsConfigurationKey)
            .Get<List<string>>() ?? throw new NotFoundPatternsException();
        _imageService = imageService;
        _logger = logger;
    }

    /// <inheritdoc />
    public List<string> GetImeiTextFromImage(MemoryStream memoryStreamImage)
    {
        MemoryStream adjustStreamImage = _imageService.AdjustContrast(memoryStreamImage, 50);
        string recognizedText = RecognizedTextFromImage(adjustStreamImage);
        return ExtractImeiFromText(recognizedText);
    }

    /// <summary>
    /// Получить текст с изображения.
    /// </summary>
    /// <param name="memoryStreamImage">Изображение.</param>
    /// <returns>Текст.</returns>
    private string RecognizedTextFromImage(MemoryStream memoryStreamImage)
    {
        using var engine = new TesseractEngine(
            TesseractTessdataPath, 
            TesseractLanguageName, 
            EngineMode.Default);
        using Pix img = Pix.LoadFromMemory(memoryStreamImage.ToArray());
        using Page recognizedPage = engine.Process(img, PageSegMode.Auto);
        string recognizedText = recognizedPage.GetText();
        
        _logger.LogInformation(recognizedText);
        
        return recognizedText;
    }

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

        return result.Distinct().ToList();
    }
}