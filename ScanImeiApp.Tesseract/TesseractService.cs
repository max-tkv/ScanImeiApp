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

    private const string TesseractLanguageName = "eng";
    private const string TesseractTessdataPath = "tessdata";

    public TesseractService(ILogger<TesseractService> logger)
    {
        _logger = logger;
        _engine = new TesseractEngine(
            TesseractTessdataPath, 
            TesseractLanguageName, 
            EngineMode.Default);
    }
    
    /// <inheritdoc />
    public string Recognize(MemoryStream memoryStreamImage, bool unspaced)
    {
        using Pix img = Pix.LoadFromMemory(memoryStreamImage.ToArray());
        using Page recognizedPage = _engine.Process(img, PageSegMode.Auto);
        string result = recognizedPage.GetText();
        
        if (unspaced)
            result = result.Replace(" ", "").Trim();

        return result;
    }
    
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
            _logger.LogInformation("Dispose!!!!!!!!!");
            _engine.Dispose();
        }

        _disposed = true;
    }
    
    ~TesseractService()
    {
        Dispose(false);
    }

    #endregion
}