namespace ScanImeiApp.Abstractions;

/// <summary>
/// Интерфейс представляющий возможность сканирования изображения на наличие IMEI при помощи OCR Teseract.
/// </summary>
public interface IScanImeiTextService
{
    /// <summary>
    /// Получить IMEI с изображения.
    /// </summary>
    /// <param name="originalImage">Изображение для сканирования.</param>
    /// <param name="imageName">Имя изображения.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Список найденных IMEI.</returns>
    Task<List<string>> GetImeiTextFromImageAsync(
        MemoryStream originalImage, 
        string imageName, 
        CancellationToken cancellationToken);
}