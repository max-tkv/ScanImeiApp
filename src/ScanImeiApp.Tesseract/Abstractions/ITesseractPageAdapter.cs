using Tesseract;

namespace ScanImeiApp.Tesseract.Abstractions;

/// <summary>
/// Интерфейс представляет описание адаптера для класса <see cref="Page" />.
/// </summary>
public interface ITesseractPageAdapter : IDisposable
{
    /// <summary>
    /// Получите среднее значение уверенности в процентах от распознанного текста.
    /// </summary>
    /// <returns>Процент.</returns>
    float GetMeanConfidence();
    
    /// <summary>
    /// Получает содержимое страницы в виде текста.
    /// </summary>
    /// <returns></returns>
    string GetText();
}