using ScanImeiApp.Abstractions;
using ScanImeiApp.Options;

namespace ScanImeiApp.Services.ModificationImage;

/// <summary>
/// Класс представляет обработчик увеличения резкости изображения.
/// </summary>
public class ModifierImageAdjustSharpness : IModifierImage
{
    private readonly IImageService _imageService;
    private readonly AppOptions _appOptions;

    public ModifierImageAdjustSharpness(
        AppOptions appOptions,
        IImageService imageService)
    {
        _imageService = imageService;
        _appOptions = appOptions;
    }
    
    /// <inheritdoc />
    public async Task<MemoryStream> ModifyImageAsync(
        MemoryStream memoryStreamImage, 
        string imageName, 
        CancellationToken cancellationToken)
    {
        return await _imageService.AdjustSharpnessAsync(
            memoryStreamImage, 
            imageName, 
            _appOptions.ImageOptions.Sharpness, 
            cancellationToken);
    }
}