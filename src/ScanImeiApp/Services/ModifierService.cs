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
        string imageName, 
        string recognizerName,
        IReadOnlyCollection<ModificationImageType> recognizerModificationTypes, 
        CancellationToken cancellationToken)
    {
        MemoryStream resultImage = new MemoryStream(originalImage.ToArray());
        foreach (var recognizerModificationType in recognizerModificationTypes)
        {
            if (recognizerModificationType == ModificationImageType.Original)
            {
                continue;
            }
                
            IModifierImage modifierImage = _modificationImageFactory.Create(recognizerModificationType);
            resultImage = await modifierImage.ModifyImageAsync(
                resultImage, 
                imageName, 
                cancellationToken);
        }

        await _imageService.SaveImageAsync(
            resultImage, 
            $"{recognizerName}-{imageName}", 
            cancellationToken);

        return resultImage;
    }
}