using System.Reflection;
using Ardalis.GuardClauses;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ScanImeiApp.Abstractions;
using Tesseract;

namespace ScanImeiApp.Tesseract.Extensions;

/// <summary>
/// Класс регистрации Tesseract.
/// </summary>
public static class RegisterTesseractDependenciesExtension
{
    
    private const string TesseractLanguageName = "eng";
    private const string TesseractTessdataDirectory = "/tessdata";
    private const string ConfigTessdataFilePath = "/tessdata/configs/engine";
    
    /// <summary>
    /// Зарегистрировать Tesseract.
    /// </summary>
    /// <param name="serviceCollection">Коллекция сервисов.</param>
    /// <returns>Коллекция сервисов.</returns>
    public static IServiceCollection RegisterTesseract(this IServiceCollection serviceCollection)
    {
        Guard.Against.Null(serviceCollection, nameof(serviceCollection));

        return serviceCollection
            .AddScoped<ITesseractService, TesseractService>(
                serviceProvider =>
                {
                    var logger = serviceProvider.GetRequiredService<ILogger<TesseractService>>();
                    return new TesseractService(
                        new TesseractEngine(
                            GetTessdataDirectoryPath(logger),
                            TesseractLanguageName,
                            EngineMode.LstmOnly,
                            GetConfigDirectoryPath(logger)));
                });
    }
    
    #region Приватные методы
    
    /// <summary>
    /// Получить путь до каталога с tessdata.
    /// </summary>
    /// <returns>Путь.</returns>
    private static string GetTessdataDirectoryPath(ILogger<TesseractService> logger)
    {
        string runDir = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location)!;
        string tessdataDir = string.Concat(runDir, TesseractTessdataDirectory);
        logger.LogDebug($"Каталога с tessdata: {tessdataDir}");
        return tessdataDir;
    }
    
    /// <summary>
    /// Получить путь до конфигурации с tessdata.
    /// </summary>
    /// <returns>Путь.</returns>
    private static string GetConfigDirectoryPath(ILogger<TesseractService> logger)
    {
        string runDir = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location)!;
        string tessdataDir = string.Concat(runDir, ConfigTessdataFilePath);
        logger.LogDebug($"Каталога с config: {tessdataDir}");
        return tessdataDir;
    }

    #endregion
}