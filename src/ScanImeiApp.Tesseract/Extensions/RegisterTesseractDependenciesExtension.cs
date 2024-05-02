using System.Reflection;
using Ardalis.GuardClauses;
using Microsoft.Extensions.DependencyInjection;
using ScanImeiApp.Abstractions;
using ScanImeiApp.Tesseract.Abstractions;
using ScanImeiApp.Tesseract.Services;
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
            .AddScoped<ITesseractService, TesseractService>()
            .AddScoped<ITesseractPixService, TesseractPixService>()
            .AddScoped<ITesseractEngineAdapter, TesseractEngineAdapter>(
                _ => new TesseractEngineAdapter(
                    new TesseractEngine(
                        GetTessdataDirectoryPath(),
                        TesseractLanguageName,
                        EngineMode.LstmOnly,
                        GetConfigDirectoryPath())));
    }
    
    #region Приватные методы
    
    /// <summary>
    /// Получить путь до каталога с tessdata.
    /// </summary>
    /// <returns>Путь.</returns>
    private static string GetTessdataDirectoryPath()
    {
        string runDir = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location)!;
        return string.Concat(runDir, TesseractTessdataDirectory);
    }
    
    /// <summary>
    /// Получить путь до конфигурации с tessdata.
    /// </summary>
    /// <returns>Путь.</returns>
    private static string GetConfigDirectoryPath()
    {
        string runDir = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location)!;
        return string.Concat(runDir, ConfigTessdataFilePath);
    }

    #endregion
}