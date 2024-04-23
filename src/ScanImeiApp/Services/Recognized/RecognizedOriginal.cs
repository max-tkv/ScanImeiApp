using Microsoft.Extensions.Logging;
using ScanImeiApp.Abstractions;
using ScanImeiApp.Lookups;
using ScanImeiApp.Options;
using ScanImeiApp.Tesseract.Abstractions;

namespace ScanImeiApp.Services.Recognized;

/// <summary>
/// Класс представляет обработчик распознавания с оригинального изображения.
/// </summary>
public class RecognizedOriginal : RecognizedBase, IRecognized
{
    public RecognizedOriginal(
        AppOptions appOptions, 
        ITesseractService tesseractService, 
        ILogger<RecognizedBase> logger,
        IRegexService regexService) : base(
        appOptions, 
        tesseractService, 
        logger,
        regexService)
    {
    }
    
    /// <inheritdoc />
    public async Task<List<string>> RecognizeImeiAsync(
        MemoryStream memoryStreamImage,
        string imageName,
        CancellationToken cancellationToken)
    {
        List<string> recognized = RecognizedAndExtractedImei(
            memoryStreamImage, 
            imageName, 
            RecognizedImageType.Original);
        return await Task.FromResult(recognized);
    }
}