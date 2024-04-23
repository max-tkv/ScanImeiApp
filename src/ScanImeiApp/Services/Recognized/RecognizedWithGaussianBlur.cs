using Microsoft.Extensions.Logging;
using ScanImeiApp.Abstractions;
using ScanImeiApp.Lookups;
using ScanImeiApp.Options;
using ScanImeiApp.Tesseract.Abstractions;

namespace ScanImeiApp.Services.Recognized;

/// <summary>
/// Класс представляет обработчик распознавания с изображения к которому применена бинаризация.
/// </summary>
public class RecognizedWithGaussianBlur : RecognizedBase, IRecognized
{
    private readonly IImageService _imageService;

    public RecognizedWithGaussianBlur(
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
        var binaryzationImage = await _imageService.GaussianBlurAsync(
            memoryStreamImage, 
            imageName, 
            _appOptions.ImageSettings.GaussianBlur,
            cancellationToken);
        return RecognizedAndExtractedImei(
            binaryzationImage, 
            imageName, 
            RecognizedImageType.GaussianBlur);
    }
}