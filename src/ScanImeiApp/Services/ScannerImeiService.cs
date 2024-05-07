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
    private readonly ILogger<ScannerImeiService> _logger;
    private readonly IImeiService _imeiService;
    private readonly IRecognizerTextService _recognizerTextService;
    private readonly IModifierService _modifierService;

    public ScannerImeiService( 
        IImageService imageService, 
        AppOptions appOptions,
        ILogger<ScannerImeiService> logger,
        IImeiService imeiService,
        IRecognizerTextService recognizerTextService,
        IModifierService modifierService)
    {
        _imageService = imageService;
        _appOptions = appOptions;
        _logger = logger;
        _imeiService = imeiService;
        _recognizerTextService = recognizerTextService;
        _modifierService = modifierService;
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
    /// <param name="imageName">Имя изображения.</param>
    /// <param name="image">Изображения.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Список IMEI.</returns>
    private async Task<List<string>> GetRecognizedImeiByOptionsAsync(
        string imageName, 
        MemoryStream image,
        CancellationToken cancellationToken)
    {
        var recognizeResults = new List<RecognizeResult>();
        IEnumerable<ModificationOptions> modifications = _appOptions.Modifications.Distinct();
        foreach (var recognizer in modifications)
        {
            var recognizeResult = await ApplyModifyImageAndRecognizeTextAsync(
                imageName, 
                image, 
                recognizer, 
                cancellationToken);
            recognizeResults.Add(recognizeResult);
        }
        
        return await _imeiService.FindImeiToRecognizeResultsAsync(
            recognizeResults, 
            cancellationToken);;
    }

    /// <summary>
    /// Выполнить модификацию изображения и извлечение текста.
    /// </summary>
    /// <param name="imageName">Имя изображения.</param>
    /// <param name="image">Изображение.</param>
    /// <param name="modification">Модификатор изображения.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Результат распознавания.</returns>
    private async Task<RecognizeResult> ApplyModifyImageAndRecognizeTextAsync(
        string imageName, 
        MemoryStream image,
        ModificationOptions modification, 
        CancellationToken cancellationToken)
    {
        try
        {
            MemoryStream modifyImage = await _modifierService.ApplyModifyImageAsync(
                image,
                imageName, 
                modification.Name,
                modification.ModificationTypes, 
                cancellationToken);
            return _recognizerTextService.RecognizeText(
                modifyImage, 
                imageName, 
                modification.Name);
        }
        catch (Exception e)
        {
            _logger.LogWarning($"Имя изображения: {imageName}\n" +
                               $"Тип изменения: {modification.Name}.\n" +
                               $"Ошибка при изменении или обработки измененного изображения.\n" +
                               $"Описание: {e.GetExceptionMessage()}");
        }

        return null!;
    }

    #endregion
}