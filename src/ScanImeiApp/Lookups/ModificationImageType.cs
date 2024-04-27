namespace ScanImeiApp.Lookups;

/// <summary>
/// Перечисление представляет типы изменения изображения. 
/// </summary>
public enum ModificationImageType
{
    // Оригинальное.
    Original = 1,
    
    // Увеличение контрастности.
    Contrast = 2,
    
    // Увеличение резкости.
    Sharpness = 3,
    
    // Бинаризация.
    Binaryzation = 4,
    
    // Гауссово размытие.
    GaussianBlur = 5,
    
    // Увеличение размера.
    Resize = 6
}