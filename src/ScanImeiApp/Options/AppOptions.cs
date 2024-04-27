namespace ScanImeiApp.Options;

public class AppOptions
{
    /// <summary>
    /// Признак устанавливает разрешение сохранять изображения.
    /// </summary>
    public bool AllowSavedImage { get; set; }
    
    /// <summary>
    /// Настройки изображения перед изъятием текста.
    /// </summary>
    public ImageOptions ImageOptions { get; set; }
    
    /// <summary>
    /// Набор обязательного текста в распознанном тексте перед поиском IMEI.
    /// </summary>
    public IReadOnlyCollection<string> RequiredTextImei { get; set; }
    
    /// <summary>
    /// Паттерны поиска IMEI.
    /// </summary>
    public IReadOnlyCollection<string> Patterns { get; set; }
    
    /// <summary>
    /// Включенные обработчики распознавания текста.
    /// </summary>
    public IReadOnlyCollection<RecognizerOptions> Recognizers { get; set; }
}