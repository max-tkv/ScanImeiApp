namespace ScanImeiApp.Lookups;

/// <summary>
/// Перечисление представляет типы изменения изображения перед извлечением текста. 
/// </summary>
public enum RecognizeTextImageType
{
    // Оригинальное.
    Original = 1,
    
    // С увеличенной контрастностью.
    Contrast = 2,
    
    // С увеличенной резкостью.
    Sharpness = 3,
    
    // Бинаризация.
    Binaryzation = 4,
    
    // Гауссово размытие.
    GaussianBlur = 5,
    
    // Увиличение.
    Resize = 6
}