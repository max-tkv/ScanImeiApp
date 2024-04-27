using ScanImeiApp.Models;

namespace ScanImeiApp.Abstractions;

/// <summary>
/// Интерфейс представляет описание класса для работы с IMEI.
/// </summary>
public interface IImeiService
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    Task<List<string>> GetFromTextAsync(
        List<RecognizeResult> recognizeResults, 
        CancellationToken cancellationToken);
}