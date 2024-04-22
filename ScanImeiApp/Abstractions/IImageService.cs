namespace ScanImeiApp.Abstractions;

/// <summary>
/// Интерфейс представляющий возможность работы с изображениями.
/// </summary>
public interface IImageService
{
    /// <summary>
    /// Увеличить контрастность изображения. 
    /// </summary>
    /// <remarks>
    /// Значение 0 создаст изображение, которое будет полностью серым. Значение 1 оставляет входные данные без изменений.
    /// Другие значения являются линейными множителями эффекта. Допускаются значения, превышающие 1, что позволяет получить более контрастные результаты.
    /// </remarks>
    /// <param name="originalImage">Изображение.</param>
    /// <param name="contrast">Величина контрастности.</param>
    /// <returns>Изображение.</returns>
    MemoryStream AdjustContrast(MemoryStream originalImage, float contrast);
}