namespace ScanImeiApp.Models;

/// <summary>
/// Модель с описанием ошибки.
/// </summary>
public class ErrorResponse
{
    /// <summary>
    /// Описание ошибки.
    /// </summary>
    public string Error { get; set; }
    
    public ErrorResponse(string error)
    {
        Error = error;
    }
}