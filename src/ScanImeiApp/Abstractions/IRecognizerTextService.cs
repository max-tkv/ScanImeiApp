using ScanImeiApp.Models;

namespace ScanImeiApp.Abstractions;

/// <summary>
/// Интерфейс представляет описание сервиса по распознаванию текста на изображении.
/// </summary>
public interface IRecognizerTextService
{
    /// <summary>
    /// Получить текст с изображения.
    /// </summary>
    /// <param name="imageName">Имя изображения.</param>
    /// <param name="image">Изображение.</param>
    /// <param name="recognizerName">Имя распознавателя.</param>
    /// <returns>Результат распознавания.</returns>
    public RecognizeResult RecognizeText(
        MemoryStream image,
        string? imageName,
        string? recognizerName);
}