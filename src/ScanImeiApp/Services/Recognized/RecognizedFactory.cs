using ScanImeiApp.Abstractions;
using ScanImeiApp.Lookups;

namespace ScanImeiApp.Services.Recognized;

/// <summary>
/// Класс представляет фабрику обработчиков распознавания текста на картинки.
/// </summary>
public class RecognizedFactory : IRecognizedFactory
{
    private readonly Dictionary<RecognizedImageType, Func<IRecognized>> recognizedFactories = new();

    /// <inheritdoc />
    public IRecognized Create(RecognizedImageType recognizedImageType)
    {
        if (TryGetRecognizedFactory(recognizedImageType, out var recognizedImageFactory))
        {
            return recognizedImageFactory.Invoke();
        }

        throw new InvalidOperationException($"Не найден обработчик для распознавания текста." +
                                            $"Тип изображения: {recognizedImageType}");
    }

    /// <summary>
    /// Добавить обработчик распознования текста.
    /// </summary>
    /// <param name="recognizedImageType">Тип создания бонусного продукта.</param>
    /// <param name="recognized">Обработчик распознования текста.</param>
    /// <returns>Фабрика обработчиков распознавания текста.</returns>
    public RecognizedFactory AddRecognized(
        RecognizedImageType recognizedImageType,
        Func<IRecognized> recognized)
    {
        recognizedFactories.Add(recognizedImageType, recognized);
        return this;
    }

    #region Приватные методы
    
    private bool TryGetRecognizedFactory(
        RecognizedImageType recognizedImageType,
        out Func<IRecognized> recognized) =>
        recognizedFactories.TryGetValue(recognizedImageType, out recognized);

    #endregion
}