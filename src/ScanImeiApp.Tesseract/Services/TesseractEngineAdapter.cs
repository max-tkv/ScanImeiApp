using ScanImeiApp.Tesseract.Abstractions;
using Tesseract;

namespace ScanImeiApp.Tesseract.Services;

/// <summary>
/// Адаптера для класса <see cref="TesseractEngine" />.
/// </summary>
public class TesseractEngineAdapter : ITesseractEngineAdapter, IDisposable
{
    private bool _disposed;
    private readonly object _lockObject = new object();
    private readonly TesseractEngine _engine;

    public TesseractEngineAdapter(TesseractEngine engine)
    {
        _engine = engine;
    }

    /// <inheritdoc />
    public ITesseractPageAdapter Process(Pix img, PageSegMode mode)
    {
        lock (_lockObject)
        {
            Page page = _engine.Process(img, mode);
            return new TesseractPageAdapter(page);
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
    
    ~TesseractEngineAdapter()
    {
        Dispose(false);
    }

    #endregion
}