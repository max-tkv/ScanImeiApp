using ScanImeiApp.Tesseract.Abstractions;
using Tesseract;

namespace ScanImeiApp.Tesseract.Services;

/// <summary>
/// Адаптер для класса <see cref="Page" />.
/// </summary>
public class TesseractPageAdapter : ITesseractPageAdapter
{
    private readonly Page _page;
    
    public TesseractPageAdapter(Page page)
    {
        _page = page;
    }

    /// <inheritdoc />
    public float GetMeanConfidence() => 
        _page.GetMeanConfidence();

    /// <inheritdoc />
    public string GetText() =>
        _page.GetText();

    /// <inheritdoc />
    public void Dispose()
    {
        _page.Dispose();
    }
}