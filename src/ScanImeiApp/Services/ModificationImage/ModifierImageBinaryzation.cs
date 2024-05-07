using ScanImeiApp.Abstractions;
using ScanImeiApp.Options;

namespace ScanImeiApp.Services.ModificationImage;

/// <summary>
/// Класс представляет обработчик бинаризации изображения.
/// </summary>
public class ModifierImageBinaryzation : IModifierImage
{
    private readonly IImageService _imageService;
    private readonly AppOptions _appOptions;

    public ModifierImageBinaryzation(
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
        return await _imageService.BinaryzationAsync(
            memoryStreamImage, 
            imageName, 
            _appOptions.ImageOptions.Binaryzation,
            cancellationToken);
    }
}