using ScanImeiApp.Lookups;

namespace ScanImeiApp.Options;

/// <summary>
/// Модель параметров изменений изображения перед распознаванием текста.
/// </summary>
public class RecognizerOptions
{
    /// <summary>
    /// Имя модификаций изображения.
    /// </summary>
    public required string Name { get; set; }
    
    /// <summary>
    /// Список модификаций.
    /// </summary>
    public required IReadOnlyCollection<ModificationImageType> ModificationTypes { get; set; }
}