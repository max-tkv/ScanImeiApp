using ScanImeiApp.Abstractions;
using ScanImeiApp.Lookups;

namespace ScanImeiApp.Services.ModificationImage;

/// <summary>
/// Класс представляет фабрику обработчиков распознавания текста на картинки.
/// </summary>
public class ModifierImageFactory : IModificationImageFactory
{
    private readonly Dictionary<ModificationImageType, Func<IModifierImage>> modificationImageFactories = new();

    /// <inheritdoc />
    public IModifierImage Create(ModificationImageType modificationImageType)
    {
        if (TryGetModificationImageFactory(modificationImageType, out var modificationImageFactory))
        {
            return modificationImageFactory.Invoke();
        }

        throw new InvalidOperationException($"Не найден обработчик для изменения изображения. " +
                                            $"Тип изменения: {modificationImageType}");
    }

    /// <summary>
    /// Добавить обработчик изменения изображения.
    /// </summary>
    /// <param name="modificationImageType">Тип изменения.</param>
    /// <param name="modificationImage">Обработчик изменения изображения.</param>
    /// <returns>Фабрика обработчиков изменения изображений.</returns>
    public ModifierImageFactory AddModifierImage(
        ModificationImageType modificationImageType,
        Func<IModifierImage> modificationImage)
    {
        modificationImageFactories.Add(modificationImageType, modificationImage);
        return this;
    }

    #region Приватные методы
    
    private bool TryGetModificationImageFactory(
        ModificationImageType modificationImageType,
        out Func<IModifierImage> modificationImage) =>
        modificationImageFactories.TryGetValue(modificationImageType, out modificationImage);

    #endregion
}