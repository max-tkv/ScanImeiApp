namespace ScanImeiApp.Abstractions;

/// <summary>
/// Интерфейс представляющий возможность сканирования изображения на наличие IMEI в виде текста.
/// </summary>
public interface IScanImeiTextService
{
    /// <summary>
    /// Получить текстовый IMEI с изображения.
    /// </summary>
    /// <param name="memoryStreamImage">Изображение для сканирования.</param>
    /// <returns>IMEI</returns>
    List<string> GetImeiTextFromImage(MemoryStream memoryStreamImage);
}