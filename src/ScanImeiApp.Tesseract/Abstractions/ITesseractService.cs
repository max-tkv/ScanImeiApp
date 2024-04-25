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
    /// <returns>Текст.</returns>
    string Recognize(MemoryStream memoryStreamImage);
}