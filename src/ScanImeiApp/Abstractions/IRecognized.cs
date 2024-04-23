namespace ScanImeiApp.Abstractions;

/// <summary>
/// Интерфейс представляет описание обработчика распознавания текста на картинки.
/// </summary>
public interface IRecognized
{
    /// <summary>
    /// Выполнить распознавание IMEI. 
    /// </summary>
    /// <param name="memoryStreamImage"></param>
    /// <param name="imageName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<string>> RecognizeImeiAsync(
        MemoryStream memoryStreamImage,
        string imageName,
        CancellationToken cancellationToken);
}