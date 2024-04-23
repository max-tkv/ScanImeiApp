using ScanImeiApp.Abstractions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;

namespace ScanImeiApp.Services;

/// <summary>
/// Клссс представляющий возможность работы с изображениями.
/// </summary>
public class ImageService : IImageService
{
    /// <inheritdoc />
    public MemoryStream AdjustContrast(MemoryStream originalImage, float contrast)
    {
        using var image = Image.Load(originalImage.ToArray());
        image.Mutate(x => x.Contrast(contrast));
        
        MemoryStream adjustedImageStream = new MemoryStream();
        image.Save(adjustedImageStream, new PngEncoder());

        adjustedImageStream.Seek(0, SeekOrigin.Begin);

        return adjustedImageStream;
    }
}