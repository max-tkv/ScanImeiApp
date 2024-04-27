using ScanImeiApp.Lookups;

namespace ScanImeiApp.Abstractions;

/// <summary>
/// Интерфейс представляет описание фабрики обработчиков распознавания текста на картинки.
/// </summary>
public interface IRecognizeTextFactory
{
    /// <summary>
    /// Создать обработчик распознавания текста.
    /// </summary>
    /// <param name="recognizeTextImageType">Типы изображения.</param>
    /// <returns>Обработчик распознавания текста</returns>
    IRecognizeText Create(RecognizeTextImageType recognizeTextImageType);
}