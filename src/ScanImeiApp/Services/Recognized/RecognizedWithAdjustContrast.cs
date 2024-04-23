using Microsoft.Extensions.Logging;
using ScanImeiApp.Abstractions;
using ScanImeiApp.Lookups;
using ScanImeiApp.Options;
using ScanImeiApp.Tesseract.Abstractions;

namespace ScanImeiApp.Services.Recognized;

/// <summary>
/// Класс представляет обработчик распознавания с изображения у которого увеличена контрастность.
/// </summary>
public class RecognizedWithAdjustContrast : RecognizedBase, IRecognized
{
    private readonly IImageService _imageService;

    public RecognizedWithAdjustContrast(
        AppOptions appOptions, 
        ITesseractService tesseractService, 
        ILogger<RecognizedBase> logger,
        IImageService imageService,
        IRegexService regexService) : base(
        appOptions, 
        tesseractService, 
        logger,
        regexService)
    {
        _imageService = imageService;
    }
    
    /// <inheritdoc />
    public async Task<List<string>> RecognizeImeiAsync(
        MemoryStream memoryStreamImage, 
        string imageName, 
        CancellationToken cancellationToken)
    {
        var adjustStreamImage = await _imageService.AdjustContrastAsync(
            memoryStreamImage, 
            imageName, 
            _appOptions.ImageSettings.Contrast, 
            cancellationToken);
        return RecognizedAndExtractedImei(
            adjustStreamImage, 
            imageName, 
            RecognizedImageType.Contrast);
    }
}