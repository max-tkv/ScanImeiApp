namespace ScanImeiApp.Models;

/// <summary>
/// Модель результата на запрос сканирования.
/// </summary>
public class ImeiResult
{
    /// <summary>
    /// Наименование изображения.
    /// </summary>
    public string ImageName { get; set; }
    
    /// <summary>
    /// Список IMEI.
    /// </summary>
    public List<string> Imei { get; set; }
}