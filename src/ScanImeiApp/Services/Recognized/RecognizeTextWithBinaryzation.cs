using Microsoft.Extensions.Logging;
using ScanImeiApp.Abstractions;
using ScanImeiApp.Lookups;
using ScanImeiApp.Models;
using ScanImeiApp.Options;

namespace ScanImeiApp.Services.Recognized;

/// <summary>
/// Класс представляет обработчик распознавания с изображения к которому применена бинаризация.
/// </summary>
public class RecognizeTextWithBinaryzation : RecognizeTextBase, IRecognizeText
{
    private readonly IImageService _imageService;

    public RecognizeTextWithBinaryzation(
        AppOptions appOptions, 
        ITesseractService tesseractService, 
        ILogger<RecognizeTextBase> logger,
        IImageService imageService) : base(
        appOptions, 
        tesseractService, 
        logger)
    {
        _imageService = imageService;
    }
    
    /// <inheritdoc />
    public async Task<RecognizeResult> RecognizeTextAsync(
        MemoryStream memoryStreamImage, 
        string imageName, 
        CancellationToken cancellationToken)
    {
        var resultImage = await _imageService.BinaryzationAsync(
            memoryStreamImage, 
            imageName, 
            _appOptions.ImageSettings.Binaryzation,
            cancellationToken);
        return RecognizeText(
            resultImage, 
            imageName, 
            RecognizeTextImageType.Binaryzation);
    }
}