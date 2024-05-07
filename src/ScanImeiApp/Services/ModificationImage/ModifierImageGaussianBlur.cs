using ScanImeiApp.Abstractions;
using ScanImeiApp.Options;

namespace ScanImeiApp.Services.ModificationImage;

/// <summary>
/// Класс представляет обработчик применения размытие Гауса к изображению.
/// </summary>
public class ModifierImageGaussianBlur : IModifierImage
{
    private readonly IImageService _imageService;
    private readonly AppOptions _appOptions;

    public ModifierImageGaussianBlur(
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
        return await _imageService.GaussianBlurAsync(
            memoryStreamImage, 
            imageName, 
            _appOptions.ImageOptions.GaussianBlur,
            cancellationToken);
    }
}