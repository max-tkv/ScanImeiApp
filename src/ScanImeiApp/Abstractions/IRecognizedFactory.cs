using ScanImeiApp.Lookups;

namespace ScanImeiApp.Abstractions;

/// <summary>
/// Интерфейс представляет описание фабрики обработчиков распознавания текста на картинки.
/// </summary>
public interface IRecognizedFactory
{
    /// <summary>
    /// Создать обработчик распознавания текста.
    /// </summary>
    /// <param name="recognizedImageType">Типы изображения.</param>
    /// <returns>Обработчик распознавания текста</returns>
    IRecognized Create(RecognizedImageType recognizedImageType);
}