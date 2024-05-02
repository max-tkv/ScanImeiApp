using Ardalis.GuardClauses;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ScanImeiApp.Abstractions;
using ScanImeiApp.Exceptions;
using ScanImeiApp.Lookups;
using ScanImeiApp.Options;
using ScanImeiApp.Services;
using ScanImeiApp.Services.ModificationImage;

namespace ScanImeiApp.Extensions;

/// <summary>
/// Класс с настройками для подключения доменного уровня приложения.
/// </summary>
public static class RegisterDomainDependenciesExtension
{
    public static IServiceCollection RegisterDomain(
        this IServiceCollection serviceCollection, 
        IConfiguration configuration)
    {
        Guard.Against.Null(serviceCollection, nameof(serviceCollection));

        return serviceCollection
            .RegisterAppOptions(configuration)
            .RegisterModifierImageFactoryHandlers()
            .RegisterModifierImageFactory()
            .AddScoped<IScannerImeiService, ScannerImeiService>()
            .AddScoped<IImageService, ImageService>()
            .AddScoped<IRegexService, RegexService>()
            .AddScoped<IImeiService, ImeiService>()
            .AddScoped<IRecognizerTextService, RecognizerTextService>()
            .AddScoped<IModifierService, ModifierService>()
            .AddScoped<IImeiValidationService, ImeiValidationService>();
    }

    #region Приватные методы

    private static IServiceCollection RegisterAppOptions(
        this IServiceCollection serviceCollection, 
        IConfiguration configuration)
    {
        var appOptions = configuration
            .GetRequiredSection("App")
            .Get<AppOptions>() ?? throw new NotFoundAppOptionsException();
        return serviceCollection.AddSingleton(appOptions);
    }
    
    private static IServiceCollection RegisterModifierImageFactoryHandlers(
        this IServiceCollection services) =>
        services
            .AddScoped<ModifierImageAdjustContrast>()
            .AddScoped<ModifierImageAdjustSharpness>()
            .AddScoped<ModifierImageBinaryzation>()
            .AddScoped<ModifierImageGaussianBlur>()
            .AddScoped<ModifierImageResize>();

    private static IServiceCollection RegisterModifierImageFactory(
        this IServiceCollection serviceCollection) =>
        serviceCollection
            .AddScoped<IModificationImageFactory>(
                container =>
                    new ModifierImageFactory()
                        .AddModifierImage(
                            ModificationImageType.Contrast,
                            container.GetRequiredService<ModifierImageAdjustContrast>)
                        .AddModifierImage(
                            ModificationImageType.Sharpness,
                            container.GetRequiredService<ModifierImageAdjustSharpness>)
                        .AddModifierImage(
                            ModificationImageType.Binaryzation,
                            container.GetRequiredService<ModifierImageBinaryzation>)
                        .AddModifierImage(
                            ModificationImageType.GaussianBlur,
                            container.GetRequiredService<ModifierImageGaussianBlur>)
                        .AddModifierImage(
                            ModificationImageType.Resize,
                            container.GetRequiredService<ModifierImageResize>));

    #endregion
}