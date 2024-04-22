namespace ScanImeiApp.Abstractions;

/// <summary>
/// Интерфейс представляющий возможность сканирования изображения на наличие IMEI при помощи OCR Teseract.
/// </summary>
public interface IScanImeiTextService
{
    /// <summary>
    /// Получить IMEI с изображения.
    /// </summary>
    /// <param name="memoryStreamImage">Изображение для сканирования.</param>
    /// <returns>Список найденных IMEI.</returns>
    List<string> GetImeiTextFromImage(MemoryStream memoryStreamImage);
}