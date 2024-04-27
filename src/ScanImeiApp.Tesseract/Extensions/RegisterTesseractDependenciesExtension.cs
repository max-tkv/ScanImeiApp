using Ardalis.GuardClauses;
using Microsoft.Extensions.DependencyInjection;
using ScanImeiApp.Abstractions;

namespace ScanImeiApp.Tesseract.Extensions;

/// <summary>
/// Класс регистрации Tesseract.
/// </summary>
public static class RegisterTesseractDependenciesExtension
{
    /// <summary>
    /// Зарегистрировать Tesseract.
    /// </summary>
    /// <param name="serviceCollection">Коллекция сервисов.</param>
    /// <returns>Коллекция сервисов.</returns>
    public static IServiceCollection RegisterTesseract(this IServiceCollection serviceCollection)
    {
        Guard.Against.Null(serviceCollection, nameof(serviceCollection));

        return serviceCollection
            .AddScoped<ITesseractService, TesseractService>();
    }
}