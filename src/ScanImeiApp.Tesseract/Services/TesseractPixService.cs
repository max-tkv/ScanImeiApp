using ScanImeiApp.Tesseract.Abstractions;
using Tesseract;

namespace ScanImeiApp.Tesseract.Services;

/// <summary>
/// Сервис для работы с классом <see cref="Pix" />.
/// </summary>
public class TesseractPixService : ITesseractPixService
{
    /// <inheritdoc />
    public Pix LoadFromMemory(MemoryStream memoryStreamImage) =>
        Pix.LoadFromMemory(memoryStreamImage.ToArray());
}