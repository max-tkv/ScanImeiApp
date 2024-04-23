namespace ScanImeiApp.Abstractions;

/// <summary>
/// Интерфейс представляет описание функционала сервиса для работы с регулярными выражениями.
/// </summary>
public interface IRegexService
{
    /// <summary>
    /// Найти по шаблонам регулярных выражений.
    /// </summary>
    /// <param name="recognizedText">Текст для анализа.</param>
    /// <param name="patterns">Шаблоны поиска.</param>
    /// <returns>Найденные строки.</returns>
    List<string> FindAndExtractedByPatterns(string recognizedText, IReadOnlyCollection<string> patterns);
}