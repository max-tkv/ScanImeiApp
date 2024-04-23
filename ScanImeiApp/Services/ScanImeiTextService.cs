using System.Text.RegularExpressions;
using ScanImeiApp.Abstractions;
using ScanImeiApp.Exceptions;
using Tesseract;

namespace ScanImeiApp.Services;

/// <summary>
/// Класс представляющий возможность сканирования изображения на наличие IMEI при помощи OCR Teseract.
/// </summary>
public class ScanImeiTextService : IScanImeiTextService, IDisposable
{
    private const string TesseractLanguageName = "eng";
    private const string TesseractTessdataPath = "tessdata";
    private const string PatternsConfigurationKey = "Patterns";
    private readonly List<string> _imeiPatterns;
    private readonly IImageService _imageService;
    private readonly ILogger<ScanImeiTextService> _logger;
    private readonly TesseractEngine _engine;
    private bool _disposed;
    
    public ScanImeiTextService(
        IConfiguration configuration, 
        IImageService imageService, 
        ILogger<ScanImeiTextService> logger)
    {
        _engine = new TesseractEngine(
            TesseractTessdataPath, 
            TesseractLanguageName, 
            EngineMode.Default);
        _imeiPatterns = configuration
            .GetRequiredSection(PatternsConfigurationKey)
            .Get<List<string>>() ?? throw new NotFoundPatternsException();
        _imageService = imageService;
        _logger = logger;
    }

    /// <inheritdoc />
    public List<string> GetImeiTextFromImage(MemoryStream memoryStreamImage, string imageName)
    {
        MemoryStream adjustStreamImage = _imageService.AdjustContrast(memoryStreamImage, 50);
        string recognizedText = RecognizedTextFromImage(adjustStreamImage);
        LogResultRecognized(imageName, recognizedText);
        return ExtractImeiFromText(recognizedText);
    }

    #region Приватные методы

    /// <summary>
    /// Получить текст с изображения.
    /// </summary>
    /// <param name="memoryStreamImage">Изображение.</param>
    /// <returns>Текст.</returns>
    private string RecognizedTextFromImage(MemoryStream memoryStreamImage)
    {
        using Pix img = Pix.LoadFromMemory(memoryStreamImage.ToArray());
        using Page recognizedPage = _engine.Process(img, PageSegMode.Auto);
        return recognizedPage.GetText().Replace(" ", "").Trim();
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
    
    private void LogResultRecognized(string imageName, string recognizedText)
    {
        _logger.LogInformation($"Результат сканирования {imageName}:\n{recognizedText}");
    }

    #endregion
    
    #region IDisposable

    /// <summary>
    /// Dispose() calls Dispose(true).
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Dispose.
    /// </summary>
    /// <param name="disposing"></param>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            _engine.Dispose();
        }

        _disposed = true;
    }
    
    ~ScanImeiTextService()
    {
        Dispose(false);
    }

    #endregion
}