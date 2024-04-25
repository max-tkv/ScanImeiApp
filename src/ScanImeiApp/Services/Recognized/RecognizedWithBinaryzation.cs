using Microsoft.Extensions.Logging;
using ScanImeiApp.Abstractions;
using ScanImeiApp.Lookups;
using ScanImeiApp.Options;
using ScanImeiApp.Tesseract.Abstractions;

namespace ScanImeiApp.Services.Recognized;

/// <summary>
/// Класс представляет обработчик распознавания с изображения к которому применена бинаризация.
/// </summary>
public class RecognizedWithBinaryzation : RecognizedBase, IRecognized
{
    private readonly IImageService _imageService;

    public RecognizedWithBinaryzation(
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
        var resultImage = await _imageService.BinaryzationAsync(
            memoryStreamImage, 
            imageName, 
            _appOptions.ImageSettings.Binaryzation,
            cancellationToken);
        return await RecognizedAndExtractedImeiAsync(
            resultImage, 
            imageName, 
            RecognizedImageType.Binaryzation,
            cancellationToken);
    }
}