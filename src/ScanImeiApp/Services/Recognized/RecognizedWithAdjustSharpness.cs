using Microsoft.Extensions.Logging;
using ScanImeiApp.Abstractions;
using ScanImeiApp.Lookups;
using ScanImeiApp.Options;
using ScanImeiApp.Tesseract.Abstractions;

namespace ScanImeiApp.Services.Recognized;

/// <summary>
/// Класс представляет обработчик распознавания с изображения у которого увеличена резкость.
/// </summary>
public class RecognizedWithAdjustSharpness : RecognizedBase, IRecognized
{
    private readonly IImageService _imageService;

    public RecognizedWithAdjustSharpness(
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
        var resultImage = await _imageService.AdjustSharpnessAsync(
            memoryStreamImage, 
            imageName, 
            _appOptions.ImageSettings.Sharpness, 
            cancellationToken);
        return await RecognizedAndExtractedImeiAsync(
            resultImage, 
            imageName, 
            RecognizedImageType.Sharpness,
            cancellationToken);
    }
}