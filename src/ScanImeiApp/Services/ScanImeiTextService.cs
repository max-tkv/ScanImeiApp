using Microsoft.Extensions.Logging;
using ScanImeiApp.Abstractions;
using ScanImeiApp.Extensions;
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
    private readonly ILogger<ScanImeiTextService> _logger;

    public ScanImeiTextService( 
        IImageService imageService, 
        AppOptions appOptions,
        IRecognizedFactory recognizedFactory,
        ILogger<ScanImeiTextService> logger)
    {
        _imageService = imageService;
        _appOptions = appOptions;
        _recognizedFactory = recognizedFactory;
        _logger = logger;
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
        var resultAll = new List<string>();
        foreach (var recognizedImageType in _appOptions.EnabledRecognized)
        {
            try
            {
                IRecognized recognized = _recognizedFactory.Create(recognizedImageType);
                IEnumerable<string> recognizeImeiList = await recognized.RecognizeImeiAsync(
                    image, 
                    imageName, 
                    cancellationToken);

                if (_appOptions.EnabledSimilarImei)
                {
                    RemoveImeiSimilar(recognizeImeiList, resultAll);
                }
                else
                {
                    resultAll.AddRange(recognizeImeiList);
                }
            }
            catch (ArgumentOutOfRangeException e)
            {
                _logger.LogWarning($"Тип изменения: {recognizedImageType}. " +
                                   $"Ошибка при обработки измененного изображения. " +
                                   $"Описание: {e.GetExceptionMessage()}");
            }
        }
        
        return resultAll.Distinct().ToList();
    }

    /// <summary>
    /// Удалить похожие IMEI.
    /// </summary>
    /// <remarks>
    /// IMEI (International Mobile Equipment Identity) — это уникальный идентификатор мобильного устройства.
    /// Каждый IMEI состоит из 15 цифр. В случае наличия двух IMEI на одном телефоне
    /// (например, у некоторых моделей смартфонов с поддержкой двух SIM-карт),
    /// они могут отличаться только в ПОСЛЕДНИХ 3 цифрах.
    /// </remarks>
    /// <param name="recognizeImeiList">IMEI для добаления.</param>
    /// <param name="resultAll">Уже добавленные IMEI.</param>
    /// <returns>True - похожи. False - не похожи.</returns>
    private void RemoveImeiSimilar(IEnumerable<string> recognizeImeiList, List<string> resultAll)
    {
        foreach (var recognizeImei in recognizeImeiList)
        {
            if (!resultAll.Any())
            {
                resultAll.Add(recognizeImei);
                continue;
            }
                    
            foreach (var resultItem in resultAll.ToList())
            {
                if (AreImeiSimilar(recognizeImei, resultItem) &&
                    resultAll.All(x => x != recognizeImei))
                {
                    resultAll.Add(recognizeImei);
                }
            }
        }
    }
    
    /// <summary>
    /// Определить похоже ли IMEI. Отличаются последние 6 символов.
    /// </summary>
    /// <param name="imei1">IMEI 1.</param>
    /// <param name="imei2">IMEI 2.</param>
    /// <returns>True - похожи. False - не похожи.</returns>
    private bool AreImeiSimilar(string imei1, string imei2)
    {
        if (imei1 == imei2)
        {
            return false;
        }
        
        if (EqualFirstPartImei(imei1, imei2, 9) & !EqualLastPartImei(imei1, imei2, 6))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Проверить первую часть IMEI (10 цыфр).
    /// </summary>
    /// <param name="imei1">IMEI 1.</param>
    /// <param name="imei2">IMEI 2.</param>
    /// <param name="numberCount">Количество последних сиволов.</param>
    /// <returns>True - равны. False - не равны.</returns>
    private static bool EqualLastPartImei(string imei1, string imei2, int numberCount)
    {
        int length = imei1.Length;
        if (imei1[(length - numberCount)..] != imei2[(length - numberCount)..])
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Прверить последню часть IMEI (5 цыфр).
    /// </summary>
    /// <param name="imei1">IMEI 1.</param>
    /// <param name="imei2">IMEI 2.</param>
    /// <param name="numberCount">Количество первых символов.</param>
    /// <returns>True - равны. False - не равны.</returns>
    private static bool EqualFirstPartImei(string imei1, string imei2, int numberCount)
    {
        for (int i = 0; i < numberCount; i++)
        {
            if (imei1[i] != imei2[i])
            {
                return false;
            }
        }

        return true;
    }

    #endregion
}