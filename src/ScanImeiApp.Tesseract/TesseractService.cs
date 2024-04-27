using ScanImeiApp.Abstractions;
using ScanImeiApp.Models;
using Tesseract;

namespace ScanImeiApp.Tesseract;

/// <summary>
/// Класс представляет возможность взаимодействия c Tesseract.
/// </summary>
public class TesseractService : ITesseractService, IDisposable
{
    private bool _disposed;
    private readonly TesseractEngine _engine;
    private readonly object _lockObject = new object();

    public TesseractService(TesseractEngine tesseractEngine)
    {
        _engine = tesseractEngine;
    }

    /// <inheritdoc />
    public RecognizeResult Recognize(MemoryStream memoryStreamImage)
    {
        using Pix img = Pix.LoadFromMemory(memoryStreamImage.ToArray());

        lock (_lockObject)
        {
            using Page recognizedPage = _engine.Process(img, PageSegMode.SingleColumn);
            float recognizedConfidence = recognizedPage.GetMeanConfidence();
            string recognizedText = recognizedPage.GetText();
            return new RecognizeResult
            {
                Confidence = recognizedConfidence,
                Text = recognizedText
            };
        }
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