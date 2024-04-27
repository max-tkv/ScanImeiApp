namespace ScanImeiApp.Abstractions;

/// <summary>
/// Интерфейс представляет описание обработчика изменения изображения.
/// </summary>
public interface IModifierImage
{
    /// <summary>
    /// Выполнить изменение изображения. 
    /// </summary>
    /// <param name="memoryStreamImage">Изображение.</param>
    /// <param name="imageName">Имя изображения.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Измененное изображение.</returns>
    Task<MemoryStream> ModifyImageAsync(
        MemoryStream memoryStreamImage,
        string imageName,
        CancellationToken cancellationToken);
}