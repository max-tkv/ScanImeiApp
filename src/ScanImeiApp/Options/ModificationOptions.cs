using ScanImeiApp.Lookups;

namespace ScanImeiApp.Options;

/// <summary>
/// Модель параметров изменений изображения перед распознаванием текста.
/// </summary>
public class ModificationOptions
{
    /// <summary>
    /// Имя модификаций изображения.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Список модификаций.
    /// </summary>
    public IReadOnlyCollection<ModificationImageType> ModificationTypes { get; set; } =
        Array.Empty<ModificationImageType>();
}