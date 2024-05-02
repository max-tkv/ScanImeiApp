using ScanImeiApp.Lookups;

namespace ScanImeiApp.Abstractions;

/// <summary>
/// Интерфейс представляет описание фабрики обработчиков модификаций изображения.
/// </summary>
public interface IModificationImageFactory
{
    /// <summary>
    /// Создать обработчик модификации изображения.
    /// </summary>
    /// <param name="modificationImageType">Типы изображения.</param>
    /// <returns>Обработчик распознавания текста</returns>
    IModifierImage Create(ModificationImageType modificationImageType);
}