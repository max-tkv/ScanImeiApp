using ScanImeiApp.Abstractions;
using ScanImeiApp.Lookups;

namespace ScanImeiApp.Services;

/// <summary>
/// Класс представляет сервис для применения модификации к изображению перед распознаванием текста.
/// </summary>
public class ModifierService : IModifierService
{
    private readonly IModificationImageFactory _modificationImageFactory;
    private readonly IImageService _imageService;

    public ModifierService(
        IModificationImageFactory modificationImageFactory,
        IImageService imageService)
    {
        _modificationImageFactory = modificationImageFactory;
        _imageService = imageService;
    }
    
    /// <inheritdoc />
    public async Task<MemoryStream> ApplyModifyImageAsync(
        MemoryStream originalImage,
        string? imageName, 
        string? recognizerName,
        IReadOnlyCollection<ModificationImageType> recognizerModificationTypes, 
        CancellationToken cancellationToken)
    {
        var resultImage = await ModifyImageAsync(
            originalImage, 
            imageName, 
            ModificationImageType.Original, 
            cancellationToken);
        
        foreach (var recognizerModificationType in recognizerModificationTypes)
        {
            resultImage = await ModifyImageAsync(
                originalImage, 
                imageName, 
                recognizerModificationType, 
                cancellationToken);
        }

        await _imageService.SaveImageAsync(
            resultImage, 
            $"{recognizerName}-{imageName}", 
            cancellationToken);

        return resultImage;
    }

    #region Приватные методы

    /// <summary>
    /// Выполнить модификацию изображения.
    /// </summary>
    /// <param name="originalImage">Оригинальное изображение.</param>
    /// <param name="imageName">Имя изображения.</param>
    /// <param name="modificationImageType">Тип модификации.</param>
    /// <param name="cancellationToken">Токен токен.</param>
    /// <returns>Модифицированное изображение.</returns>
    private async Task<MemoryStream> ModifyImageAsync(
        MemoryStream originalImage, 
        string? imageName, 
        ModificationImageType modificationImageType,
        CancellationToken cancellationToken)
    {
        if (modificationImageType == ModificationImageType.Original)
        {
            return originalImage;
        }
        
        IModifierImage modifierImage = _modificationImageFactory.Create(modificationImageType);
        return await modifierImage.ModifyImageAsync(
            originalImage, 
            imageName, 
            cancellationToken);
    }

    #endregion
}