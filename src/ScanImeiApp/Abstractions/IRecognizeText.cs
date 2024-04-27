using ScanImeiApp.Models;

namespace ScanImeiApp.Abstractions;

/// <summary>
/// Интерфейс представляет описание обработчика распознавания текста на картинки.
/// </summary>
public interface IRecognizeText
{
    /// <summary>
    /// Выполнить распознавание текста. 
    /// </summary>
    /// <param name="memoryStreamImage">Изображение.</param>
    /// <param name="imageName">Имя изображения.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Результат распознавания.</returns>
    Task<RecognizeResult> RecognizeTextAsync(
        MemoryStream memoryStreamImage,
        string imageName,
        CancellationToken cancellationToken);
}