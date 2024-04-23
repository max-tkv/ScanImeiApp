namespace ScanImeiApp.Tesseract.Abstractions;

/// <summary>
/// Интерфейс представляет описание сервиса взаимодействия с Tesseract.
/// </summary>
public interface ITesseractService
{
    /// <summary>
    /// Получить текст с изображения.
    /// </summary>
    /// <param name="memoryStreamImage">Изображение.</param>
    /// <param name="unspaced">Удалить все пробелы в результате.</param>
    /// <returns>Текст.</returns>
    string Recognize(MemoryStream memoryStreamImage, bool unspaced);
}