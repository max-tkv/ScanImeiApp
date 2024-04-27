namespace ScanImeiApp.Options;

/// <summary>
/// Модель параметров для изменения изображения.
/// </summary>
public class ImageOptions
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