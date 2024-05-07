namespace ScanImeiApp.Options;

public class AppOptions
{
    /// <summary>
    /// Признак устанавливает разрешение сохранять изображения.
    /// </summary>
    public bool AllowSavedImage { get; set; }
    
    /// <summary>
    /// Включить валидацию IMEI алгоритмом Луна.
    /// </summary>
    public bool EnabledVariationLuhnAlgorithm { get; set; }
    
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
    /// Включенные параметры изменения изображения перед распознаванием текста.
    /// </summary>
    public IReadOnlyCollection<ModificationOptions> Modifications { get; set; }
}