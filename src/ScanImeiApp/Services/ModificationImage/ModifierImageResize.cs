using ScanImeiApp.Abstractions;
using ScanImeiApp.Options;

namespace ScanImeiApp.Services.ModificationImage;

/// <summary>
/// Класс представляет обработчик изменения размера изображения.
/// </summary>
public class ModifierImageResize : IModifierImage
{
    private readonly IImageService _imageService;
    private readonly AppOptions _appOptions;

    public ModifierImageResize(
        AppOptions appOptions,
        IImageService imageService)
    {
        _imageService = imageService;
        _appOptions = appOptions;
    }
    
    /// <inheritdoc />
    public async Task<MemoryStream> ModifyImageAsync(
        MemoryStream memoryStreamImage, 
        string? imageName, 
        CancellationToken cancellationToken)
    {
        return await _imageService.ResizeAsync(
            memoryStreamImage, 
            imageName, 
            _appOptions.ImageOptions.ResizeDpi,
            cancellationToken);
    }
}