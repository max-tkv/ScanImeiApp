using System.Reflection;
using Microsoft.Extensions.Logging;
using ScanImeiApp.Tesseract.Abstractions;
using Tesseract;

namespace ScanImeiApp.Tesseract;

/// <summary>
/// Класс представляет возможность взаимодействия c Tesseract.
/// </summary>
public class TesseractService : ITesseractService, IDisposable
{
    private bool _disposed;
    private readonly TesseractEngine _engine;
    private readonly ILogger<TesseractService> _logger;
    private readonly object _lockObject = new object();

    private const string TesseractLanguageName = "eng";
    private const string TesseractTessdataDirectory = "/tessdata";
    private const string ConfigTessdataFilePath = "/tessdata/configs/engine.config";

    public TesseractService(ILogger<TesseractService> logger)
    {
        _logger = logger;
        _engine = new TesseractEngine(
            GetTessdataDirectoryPath(), 
            TesseractLanguageName, 
            EngineMode.Default,
            GetConfigDirectoryPath());
    }

    /// <inheritdoc />
    public string Recognize(MemoryStream memoryStreamImage, bool format)
    {
        using Pix img = Pix.LoadFromMemory(memoryStreamImage.ToArray());

        lock (_lockObject)
        {
            using Page recognizedPage = _engine.Process(img, PageSegMode.SingleColumn);   
            string result = recognizedPage.GetText();
            
            if (format)
                result = FormatRecognizeText(result);
            
            return result;
        }
    }

    #region Приватные методы

    /// <summary>
    /// Получить путь до каталога с tessdata.
    /// </summary>
    /// <returns>Путь.</returns>
    private string GetTessdataDirectoryPath()
    {
        string runDir = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location)!;
        string tessdataDir = string.Concat(runDir, TesseractTessdataDirectory);
        _logger.LogInformation($"Каталога с tessdata: {tessdataDir}");
        return tessdataDir;
    }
    
    /// <summary>
    /// Получить путь до конфигурации с tessdata.
    /// </summary>
    /// <returns>Путь.</returns>
    private string GetConfigDirectoryPath()
    {
        string runDir = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location)!;
        string tessdataDir = string.Concat(runDir, ConfigTessdataFilePath);
        _logger.LogInformation($"Каталога с config: {tessdataDir}");
        return tessdataDir;
    }

    /// <summary>
    /// Форматировать распознанный текст с изображения.
    /// </summary>
    /// <param name="recognizeText">Распознанный текст.</param>
    /// <returns>Форматированный распознанный текст.</returns>
    private static string FormatRecognizeText(string recognizeText) =>
        recognizeText
            .Replace(" ", string.Empty)
            .Replace("\n", string.Empty)
            .Trim();

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
            lock (_lockObject)
            {
                _engine.Dispose();
            }
        }

        _disposed = true;
    }
    
    ~TesseractService()
    {
        Dispose(false);
    }

    #endregion
}