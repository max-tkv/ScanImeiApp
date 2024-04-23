using Ardalis.GuardClauses;
using Microsoft.Extensions.DependencyInjection;
using ScanImeiApp.Tesseract.Abstractions;

namespace ScanImeiApp.Tesseract.Extensions;

public static class RegisterTesseractDependenciesExtension
{
    public static IServiceCollection RegisterTesseract(this IServiceCollection serviceCollection)
    {
        Guard.Against.Null(serviceCollection, nameof(serviceCollection));

        return serviceCollection
            .AddScoped<ITesseractService, TesseractService>();
    }
}