using ScanImeiApp.Lookups;

namespace ScanImeiApp.Abstractions;

/// <summary>
/// Интерфейс представляет описание сервиса для применения модификаций к изображению перед распознаванием текста.
/// </summary>
public interface IModifierService
{
    /// <summary>
    /// Применить изменения для изображения.
    /// </summary>
    /// <param name="originalImage">Изображение до изменений.</param>
    /// <param name="imageName">Имя изображения.</param>
    /// <param name="recognizerName">Имя распознавателя.</param>
    /// <param name="recognizerModificationTypes">Список типов применяемых изменений.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Измененное изображение.</returns>
    Task<MemoryStream> ApplyModifyImageAsync(
        MemoryStream originalImage,
        string imageName, 
        string recognizerName, 
        IReadOnlyCollection<ModificationImageType> recognizerModificationTypes, 
        CancellationToken cancellationToken);
}