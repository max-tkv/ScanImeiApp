namespace ScanImeiApp.Abstractions;

/// <summary>
/// Интерфейс представляет описание сервиса валидации IMEI.
/// </summary>
public interface IImeiValidationService
{
    /// <summary>
    /// Проверить все IMEI в списке и получить новый список.  
    /// </summary>
    /// <param name="imei">Список IMEI для фильрации.</param>
    /// <returns>Отфильтрованный список.</returns>
    List<string> FilterValidated(List<string> imei);
}