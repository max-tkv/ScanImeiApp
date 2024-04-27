using ScanImeiApp.Abstractions;
using ScanImeiApp.Lookups;

namespace ScanImeiApp.Services.Recognized;

/// <summary>
/// Класс представляет фабрику обработчиков распознавания текста на картинки.
/// </summary>
public class RecognizeTextFactory : IRecognizeTextFactory
{
    private readonly Dictionary<RecognizeTextImageType, Func<IRecognizeText>> recognizeTextFactories = new();

    /// <inheritdoc />
    public IRecognizeText Create(RecognizeTextImageType recognizeTextImageType)
    {
        if (TryGetRecognizeTextFactory(recognizeTextImageType, out var recognizedImageFactory))
        {
            return recognizedImageFactory.Invoke();
        }

        throw new InvalidOperationException($"Не найден обработчик для распознавания текста." +
                                            $"Тип изображения: {recognizeTextImageType}");
    }

    /// <summary>
    /// Добавить обработчик распознования текста.
    /// </summary>
    /// <param name="recognizeTextImageType">Тип создания бонусного продукта.</param>
    /// <param name="recognized">Обработчик распознования текста.</param>
    /// <returns>Фабрика обработчиков распознавания текста.</returns>
    public RecognizeTextFactory AddRecognizeText(
        RecognizeTextImageType recognizeTextImageType,
        Func<IRecognizeText> recognized)
    {
        recognizeTextFactories.Add(recognizeTextImageType, recognized);
        return this;
    }

    #region Приватные методы
    
    private bool TryGetRecognizeTextFactory(
        RecognizeTextImageType recognizeTextImageType,
        out Func<IRecognizeText> recognizeText) =>
        recognizeTextFactories.TryGetValue(recognizeTextImageType, out recognizeText);

    #endregion
}