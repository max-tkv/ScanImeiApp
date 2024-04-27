using ScanImeiApp.Abstractions;
using ScanImeiApp.Options;

namespace ScanImeiApp.Services.ModificationImage;

/// <summary>
/// Класс представляет обработчик увеличения контрастности.
/// </summary>
public class ModifierImageAdjustContrast : IModifierImage
{
    private readonly IImageService _imageService;
    private readonly AppOptions _appOptions;

    public ModifierImageAdjustContrast(
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
        return await _imageService.AdjustContrastAsync(
            memoryStreamImage, 
            imageName, 
            _appOptions.ImageOptions.Contrast, 
            cancellationToken);
    }
}