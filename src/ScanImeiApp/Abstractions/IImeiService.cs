using ScanImeiApp.Models;

namespace ScanImeiApp.Abstractions;

/// <summary>
/// Интерфейс представляет описание класса для работы с IMEI.
/// </summary>
public interface IImeiService
{
    /// <summary>
    /// Получить IMEI из результатов распознавания текста на изображении.
    /// </summary>
    /// <param name="recognizeResults">Результаты распознавания текста на изображении.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Список найденных IMEI.</returns>
    Task<List<string>> FindImeiToRecognizeResultsAsync(
        List<RecognizeResult> recognizeResults, 
        CancellationToken cancellationToken);
}