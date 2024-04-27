using Microsoft.Extensions.Logging;
using ScanImeiApp.Abstractions;
using ScanImeiApp.Lookups;
using ScanImeiApp.Models;
using ScanImeiApp.Options;

namespace ScanImeiApp.Services.Recognized;

/// <summary>
/// Класс представляет обработчик распознавания с оригинального изображения.
/// </summary>
public class RecognizeTextOriginal : RecognizeTextBase, IRecognizeText
{
    public RecognizeTextOriginal(
        AppOptions appOptions, 
        ITesseractService tesseractService, 
        ILogger<RecognizeTextBase> logger) : base(
        appOptions, 
        tesseractService, 
        logger)
    {
    }
    
    /// <inheritdoc />
    public Task<RecognizeResult> RecognizeTextAsync(
        MemoryStream memoryStreamImage,
        string imageName,
        CancellationToken cancellationToken)
    {
        RecognizeResult recognized = RecognizeText(
            memoryStreamImage, 
            imageName, 
            RecognizeTextImageType.Original);
        return Task.FromResult(recognized);
    }
}