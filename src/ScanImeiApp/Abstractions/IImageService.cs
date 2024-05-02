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
    /// <param name="imageName">Имя изображения.</param>
    /// <param name="contrast">Величина контрастности.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Изображение.</returns>
    Task<MemoryStream> AdjustContrastAsync(
        MemoryStream originalImage, 
        string imageName, 
        float contrast,
        CancellationToken cancellationToken);

    /// <summary>
    /// Увеличить резкость изображения. 
    /// </summary>
    /// <param name="originalImage">Изображение.</param>
    /// <param name="imageName">Имя изображения.</param>
    /// <param name="sharpness">Величина резкости.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Изображение.</returns>
    Task<MemoryStream> AdjustSharpnessAsync(
        MemoryStream originalImage, 
        string imageName, 
        float sharpness,
        CancellationToken cancellationToken);

    /// <summary>
    /// Применить бинаризацию.
    /// </summary>
    /// <param name="originalImage">Изображение.</param>
    /// <param name="imageName">Имя изображения.</param>
    /// <param name="threshold">Величина бинаризации.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Изображение.</returns>
    Task<MemoryStream> BinaryzationAsync(
        MemoryStream originalImage, 
        string imageName, 
        float threshold,
        CancellationToken cancellationToken);

    /// <summary>
    /// Сохранить изображения.
    /// </summary>
    /// <param name="memoryStreamImage">Изображение.</param>
    /// <param name="imageName">Имя изображения.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    Task SaveImageAsync(
        MemoryStream memoryStreamImage, 
        string imageName,
        CancellationToken cancellationToken);

    /// <summary>
    /// Удалить альфа-канал.
    /// </summary>
    /// <param name="originalImage">Изображение.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Новое изображение.</returns>
    Task<MemoryStream> RemoveAlphaChannelAsync(
        MemoryStream originalImage,
        CancellationToken cancellationToken);

    /// <summary>
    /// Применить Гауссово размытие.
    /// </summary>
    /// <param name="originalImage">Изображение.</param>
    /// <param name="imageName">Имя изображения.</param>
    /// <param name="threshold">Величина размытия.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns></returns>
    Task<MemoryStream> GaussianBlurAsync(
        MemoryStream originalImage, 
        string imageName,
        float threshold,
        CancellationToken cancellationToken);

    /// <summary>
    /// Изменить размер изображения если его DPI менее ожидаемого.
    /// </summary>
    /// <param name="originalImage">Изображение.</param>
    /// <param name="imageName">Имя изображения.</param>
    /// <param name="resizeDpi">DPI.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns></returns>
    Task<MemoryStream> ResizeAsync(
        MemoryStream originalImage,
        string imageName,
        double resizeDpi,
        CancellationToken cancellationToken);
}