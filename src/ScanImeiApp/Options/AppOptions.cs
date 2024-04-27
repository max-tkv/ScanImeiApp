using ScanImeiApp.Lookups;

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
    public ImageSettings ImageSettings { get; set; }
    
    /// <summary>
    /// Паттерны поиска IMEI.
    /// </summary>
    public IReadOnlyCollection<string> Patterns { get; set; }
    
    /// <summary>
    /// Включенные обработчики распознавания текста.
    /// </summary>
    public IReadOnlyCollection<RecognizeTextImageType> EnabledRecognized { get; set; }
}

public class ImageSettings
{
    /// <summary>
    /// Контрастность.
    /// </summary>
    public float Contrast { get; set; }
    
    /// <summary>
    /// Резкость.
    /// </summary>
    public float Sharpness { get; set; }
    
    /// <summary>
    /// Бинаризация.
    /// </summary>
    public float Binaryzation { get; set; }

    /// <summary>
    /// Гауссово размытие.
    /// </summary>
    public float GaussianBlur { get; set; }
    
    /// <summary>
    /// DPI.
    /// </summary>
    public double ResizeDpi { get; set; }
}