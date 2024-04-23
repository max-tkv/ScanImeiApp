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
            .AddScoped<IRegexService, RegexService>();
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
            .AddScoped<RecognizedOriginal>()
            .AddScoped<RecognizedWithAdjustContrast>()
            .AddScoped<RecognizedWithAdjustSharpness>()
            .AddScoped<RecognizedWithBinaryzation>()
            .AddScoped<RecognizedWithGaussianBlur>();

    private static IServiceCollection RegisterRecognizedFactory(
        this IServiceCollection serviceCollection) =>
        serviceCollection
            .AddScoped<IRecognizedFactory>(
                container =>
                    new RecognizedFactory()
                        .AddRecognized(
                            RecognizedImageType.Original,
                            container.GetRequiredService<RecognizedOriginal>)
                        .AddRecognized(
                            RecognizedImageType.Contrast,
                            container.GetRequiredService<RecognizedWithAdjustContrast>)
                        .AddRecognized(
                            RecognizedImageType.Sharpness,
                            container.GetRequiredService<RecognizedWithAdjustSharpness>)
                        .AddRecognized(
                            RecognizedImageType.Binaryzation,
                            container.GetRequiredService<RecognizedWithBinaryzation>)
                        .AddRecognized(
                            RecognizedImageType.GaussianBlur,
                            container.GetRequiredService<RecognizedWithGaussianBlur>));

    #endregion
}