using System.Reflection;
using Microsoft.Extensions.Logging;
using ScanImeiApp.Abstractions;
using ScanImeiApp.Options;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Transforms;

namespace ScanImeiApp.Services;

/// <summary>
/// Класс представляющий возможность работы с изображениями.
/// </summary>
public class ImageService : IImageService
{
    private const string ImageDirectoryName = "/Saved";
    private readonly ILogger<ImageService> _logger;
    private readonly AppOptions _appOptions;

    public ImageService(ILogger<ImageService> logger, AppOptions appOptions)
    {
        _logger = logger;
        _appOptions = appOptions;
    }
    
    /// <inheritdoc />
    public async Task<MemoryStream> AdjustContrastAsync(
        MemoryStream originalImage, 
        string? imageName, 
        float contrast,
        CancellationToken cancellationToken)
    {
        using var image = Image.Load(originalImage.ToArray());
        
        image.Mutate(x => x.Contrast(contrast));
        
        MemoryStream streamImage = new MemoryStream();
        await image.SaveAsync(streamImage, new JpegEncoder(), cancellationToken);

        streamImage.Seek(0, SeekOrigin.Begin);

        return streamImage;
    }
    
    /// <inheritdoc />
    public async Task<MemoryStream> AdjustSharpnessAsync(
        MemoryStream originalImage, 
        string? imageName, 
        float sharpness,
        CancellationToken cancellationToken)
    {
        using var image = Image.Load(originalImage.ToArray());
        
        image.Mutate(x => x.GaussianSharpen(sharpness));
        
        MemoryStream streamImage = new MemoryStream();
        await image.SaveAsync(streamImage, new JpegEncoder(), cancellationToken);

        streamImage.Seek(0, SeekOrigin.Begin);

        return streamImage;
    }
    
    /// <inheritdoc />
    public async Task<MemoryStream> BinaryzationAsync(
        MemoryStream originalImage, 
        string? imageName, 
        float threshold,
        CancellationToken cancellationToken)
    {
        using var image = Image.Load(originalImage.ToArray());
        
        image.Mutate(x => x.BinaryThreshold(threshold, Color.White, Color.Black));
        
        MemoryStream streamImage = new MemoryStream();
        await image.SaveAsync(streamImage, new JpegEncoder(), cancellationToken);

        streamImage.Seek(0, SeekOrigin.Begin);

        return streamImage;
    }

    /// <inheritdoc />
    public async Task SaveImageAsync(
        MemoryStream memoryStreamImage, 
        string imageName,
        CancellationToken cancellationToken)
    {
        if (_appOptions.AllowSavedImage)
        {
            using var image = Image.Load(memoryStreamImage.ToArray());
            await SaveImageToSpecialDirectoryAsync(image, imageName, cancellationToken);
        }
    }

    /// <inheritdoc />
    public async Task<MemoryStream> RemoveAlphaChannelAsync(
        MemoryStream originalImage,
        CancellationToken cancellationToken)
    {
        using var image = Image.Load(originalImage.ToArray());
        image.Mutate(x => x.BackgroundColor(Color.Red));
        MemoryStream streamImage = new MemoryStream();
        await image.SaveAsync(streamImage, new JpegEncoder(), cancellationToken);

        streamImage.Seek(0, SeekOrigin.Begin);

        return streamImage;
    }

    /// <inheritdoc />
    public async Task<MemoryStream> GaussianBlurAsync(
        MemoryStream originalImage, 
        string? imageName,
        float threshold,
        CancellationToken cancellationToken)
    {
        using var image = Image.Load(originalImage.ToArray());
        
        image.Mutate(x => x.GaussianBlur(threshold));
        
        MemoryStream streamImage = new MemoryStream();
        await image.SaveAsync(streamImage, new JpegEncoder(), cancellationToken);

        streamImage.Seek(0, SeekOrigin.Begin);

        return streamImage;
    }
    
    /// <inheritdoc />
    public async Task<MemoryStream> ResizeAsync(
        MemoryStream originalImage, 
        string? imageName,
        double resizeDpi,
        CancellationToken cancellationToken)
    {
        using var image = Image.Load(originalImage.ToArray());
        
        double currentDpi = image.Metadata.HorizontalResolution;
        _logger.LogDebug($"{imageName}. Текущий DPI: {currentDpi}");
        if (image.Metadata.HorizontalResolution > resizeDpi || 
            image.Metadata.HorizontalResolution <= 1)
        {
            return originalImage;
        }
        
        double resizeRatio = resizeDpi / currentDpi;
        int targetWidth = (int)Math.Round(image.Width * resizeRatio); 
        int targetHeight = (int)Math.Round(image.Height * resizeRatio);
        image.Metadata.HorizontalResolution = resizeDpi;
        image.Metadata.VerticalResolution = resizeDpi;
        
        var resampler = new BicubicResampler();
        var jpegEncoder = new JpegEncoder { Quality = 100 };
        
        image.Mutate(x => x.Resize(targetWidth, targetHeight, resampler, false));
        
        MemoryStream streamImage = new MemoryStream();
        await image.SaveAsync(streamImage, jpegEncoder, cancellationToken);

        streamImage.Seek(0, SeekOrigin.Begin);

        return streamImage;
    }


    #region Приватные методы

    /// <summary>
    /// Сохранить изображение в выделенное место.
    /// </summary>
    /// <param name="image">Изображение.</param>
    /// <param name="imageName">Имя изображения.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    private async Task SaveImageToSpecialDirectoryAsync(
        Image image, 
        string imageName, 
        CancellationToken cancellationToken)
    {
        if (!_appOptions.AllowSavedImage)
        {
            return;
        }
        
        string imageDirectoryPath = GetImageDirectoryPath();
        string imageNameWithDirectory = $"{imageDirectoryPath}/{imageName}";
        await image.SaveAsJpegAsync(imageNameWithDirectory, cancellationToken);
        _logger.LogDebug($"Изображение: {imageName}\nсохранено в:{imageNameWithDirectory}");
    }
    
    /// <summary>
    /// Получить путь до каталога сохранения файла.
    /// </summary>
    /// <returns>Путь.</returns>
    private string GetImageDirectoryPath()
    {
        string runDir = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location)!;
        string tessdataDir = string.Concat(runDir, ImageDirectoryName);

        if (!Directory.Exists(tessdataDir))
        {
            Directory.CreateDirectory(tessdataDir);
        }
        
        return tessdataDir;
    }

    #endregion
}