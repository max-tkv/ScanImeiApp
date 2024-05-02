using Tesseract;

namespace ScanImeiApp.Tesseract.Abstractions;

/// <summary>
/// Интерфейс представляет описание адаптера для класса <see cref="TesseractEngine" />.
/// </summary>
public interface ITesseractEngineAdapter
{
    /// <summary>
    /// Выполнить обнаружение текста на изображении.
    /// </summary>
    /// <param name="img">Изображение.</param>
    /// <param name="mode">Режимы анализа компоновки страницы.</param>
    /// <returns>Страница с результатом.</returns>
    ITesseractPageAdapter Process(Pix img, PageSegMode mode);
}