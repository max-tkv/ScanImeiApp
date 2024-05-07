namespace ScanImeiApp.Models;

/// <summary>
/// Класс представляет модель результата распознавания текста с изображения.
/// </summary>
public class RecognizeResult
{
    /// <summary>
    /// Имя изображения.
    /// </summary>
    public string? ImageName { get; set; }
    
    /// <summary>
    /// Распознанный текст.
    /// </summary>
    public string? Text { get; set; }
    
    /// <summary>
    /// Уровень доверия к распознаванию от OCR.
    /// </summary>
    public float Confidence { get; set; }
    
    /// <summary>
    /// Имя распознавателя.
    /// </summary>
    public string? RecognizerName { get; set; }
}