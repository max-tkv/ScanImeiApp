using Ardalis.GuardClauses;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ScanImeiApp.Abstractions;
using ScanImeiApp.Exceptions;
using ScanImeiApp.Lookups;
using ScanImeiApp.Options;
using ScanImeiApp.Services;
using ScanImeiApp.Services.Recognized;

namespace ScanImeiApp.Extensions;

public static class RegisterDomainDependenciesExtension
{
    public static IServiceCollection RegisterDomain(
        this IServiceCollection serviceCollection, 
        IConfiguration configuration)
    {
        Guard.Against.Null(serviceCollection, nameof(serviceCollection));

        return serviceCollection
            .RegisterAppOptions(configuration)
            .RegisterRecognizedFactoryHandlers()
            .RegisterRecognizedFactory()
            .AddScoped<IScanImeiTextService, ScanImeiTextService>()
            .AddScoped<IImageService, ImageService>()
            .AddScoped<IRegexService, RegexService>()
            .AddScoped<IImeiService, ImeiService>();
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
    
    private static IServiceCollection RegisterRecognizedFactoryHandlers(
        this IServiceCollection services) =>
        services
            .AddScoped<RecognizeTextOriginal>()
            .AddScoped<RecognizeTextWithAdjustContrast>()
            .AddScoped<RecognizeTextWithAdjustSharpness>()
            .AddScoped<RecognizeTextWithBinaryzation>()
            .AddScoped<RecognizeTextWithGaussianBlur>()
            .AddScoped<RecognizeTextWithResize>();

    private static IServiceCollection RegisterRecognizedFactory(
        this IServiceCollection serviceCollection) =>
        serviceCollection
            .AddScoped<IRecognizeTextFactory>(
                container =>
                    new RecognizeTextFactory()
                        .AddRecognizeText(
                            RecognizeTextImageType.Original,
                            container.GetRequiredService<RecognizeTextOriginal>)
                        .AddRecognizeText(
                            RecognizeTextImageType.Contrast,
                            container.GetRequiredService<RecognizeTextWithAdjustContrast>)
                        .AddRecognizeText(
                            RecognizeTextImageType.Sharpness,
                            container.GetRequiredService<RecognizeTextWithAdjustSharpness>)
                        .AddRecognizeText(
                            RecognizeTextImageType.Binaryzation,
                            container.GetRequiredService<RecognizeTextWithBinaryzation>)
                        .AddRecognizeText(
                            RecognizeTextImageType.GaussianBlur,
                            container.GetRequiredService<RecognizeTextWithGaussianBlur>)
                        .AddRecognizeText(
                            RecognizeTextImageType.Resize,
                            container.GetRequiredService<RecognizeTextWithResize>));

    #endregion
}