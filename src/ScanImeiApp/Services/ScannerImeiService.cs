using Microsoft.Extensions.Logging;
using ScanImeiApp.Abstractions;
using ScanImeiApp.Extensions;
using ScanImeiApp.Models;
using ScanImeiApp.Options;

namespace ScanImeiApp.Services;

/// <summary>
/// Класс представляющий возможность сканирования изображения на наличие IMEI при помощи OCR Teseract.
/// </summary>
public class ScannerImeiService : IScannerImeiService
{
    private const string OriginalImagePrefix = "original-";
    private readonly IImageService _imageService;
    private readonly AppOptions _appOptions;
    private readonly IRecognizeTextFactory _recognizeTextFactory;
    private readonly ILogger<ScannerImeiService> _logger;
    private readonly IImeiService _imeiService;

    public ScannerImeiService( 
        IImageService imageService, 
        AppOptions appOptions,
        IRecognizeTextFactory recognizeTextFactory,
        ILogger<ScannerImeiService> logger,
        IImeiService imeiService)
    {
        _imageService = imageService;
        _appOptions = appOptions;
        _recognizeTextFactory = recognizeTextFactory;
        _logger = logger;
        _imeiService = imeiService;
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
        MemoryStream changedImage = await _imageService.RemoveAlphaChannelAsync(
            originalImage, 
            cancellationToken);
        
        return await GetRecognizedImeiByOptionsAsync(
            imageName, 
            changedImage, 
            cancellationToken);
    }

    #region Приватные методы

    /// <summary>
    /// Получить IMEI по параметрам из настрояк приложения.
    /// </summary>
    /// <param name="imageName">Имя приложения.</param>
    /// <param name="image">Изображения.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Список IMEI.</returns>
    private async Task<List<string>> GetRecognizedImeiByOptionsAsync(
        string imageName, 
        MemoryStream image,
        CancellationToken cancellationToken)
    {
        var resultAll = new List<RecognizeResult>();
        foreach (var recognizedImageType in _appOptions.Recognizers)
        {
            try
            {
                IRecognizeText recognizeText = _recognizeTextFactory.Create(recognizedImageType);
                RecognizeResult recognizeResult = await recognizeText.RecognizeTextAsync(
                    image, 
                    imageName, 
                    cancellationToken);
                resultAll.Add(recognizeResult);
            }
            catch (ArgumentOutOfRangeException e)
            {
                _logger.LogWarning($"Тип изменения: {recognizedImageType}. " +
                                   $"Ошибка при обработки измененного изображения. " +
                                   $"Описание: {e.GetExceptionMessage()}");
            }
        }

        var resultImei = await _imeiService.GetFromTextAsync(resultAll, cancellationToken);
        
        return resultImei.Distinct().ToList();
    }

    

    #endregion
}