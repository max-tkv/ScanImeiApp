using Tesseract;

namespace ScanImeiApp.Tesseract.Abstractions;

/// <summary>
/// Интерфейс представляет описание сервиса для работы с классом <see cref="Pix" />.
/// </summary>
public interface ITesseractPixService
{
    /// <summary>
    /// Создать объект <see cref="Pix" />.
    /// </summary>
    /// <param name="memoryStreamImage">Изображение.</param>
    /// <returns>Объект <see cref="Pix" />.</returns>
    Pix LoadFromMemory(MemoryStream memoryStreamImage);
}