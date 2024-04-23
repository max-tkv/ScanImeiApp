using ScanImeiApp.Abstractions;
using ScanImeiApp.Options;

namespace ScanImeiApp.Services;

/// <summary>
/// Класс представляющий возможность сканирования изображения на наличие IMEI при помощи OCR Teseract.
/// </summary>
public class ScanImeiTextService : IScanImeiTextService
{
    private const string OriginalImagePrefix = "original-";
    private readonly IImageService _imageService;
    private readonly AppOptions _appOptions;
    private readonly IRecognizedFactory _recognizedFactory;

    public ScanImeiTextService( 
        IImageService imageService, 
        AppOptions appOptions,
        IRecognizedFactory recognizedFactory)
    {
        _imageService = imageService;

        _appOptions = appOptions;
        _recognizedFactory = recognizedFactory;
    }

    /// <inheritdoc />
    public async Task<List<string>> GetImeiTextFromImageAsync(
        MemoryStream originalImage, 
        string imageName, 
        CancellationToken cancellationToken)
    {
        await _imageService.SaveImageAsync(
            originalImage, 
            string.Concat(OriginalImagePrefix, imageName),
            cancellationToken);
        MemoryStream image = await _imageService.RemoveAlphaChannelAsync(
            originalImage, 
            cancellationToken);
        
        return await GetRecognizedImeiByOptionsAsync(
            imageName, 
            image, 
            cancellationToken);
    }

    #region Приватные методы

    /// <summary>
    /// Получить IMEI по параметрам из настрояк приложения.
    /// </summary>
    /// <param name="imageName"></param>
    /// <param name="image"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task<List<string>> GetRecognizedImeiByOptionsAsync(
        string imageName, 
        MemoryStream image,
        CancellationToken cancellationToken)
    {
        var result = new List<string>();
        foreach (var recognizedImageType in _appOptions.EnabledRecognized)
        {
            IRecognized recognized = _recognizedFactory.Create(recognizedImageType);
            IEnumerable<string> recognizeImeiTask = await recognized.RecognizeImeiAsync(
                image, 
                imageName, 
                cancellationToken);
            result.AddRange(recognizeImeiTask);
        }
        
        return result.Distinct().ToList();
    }

    #endregion
}