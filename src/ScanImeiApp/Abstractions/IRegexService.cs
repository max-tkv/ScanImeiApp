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
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Найденные строки.</returns>
    Task<List<string>> FindAndExtractedByPatternsAsync(
        string recognizedText, 
        IReadOnlyCollection<string>? patterns,
        CancellationToken cancellationToken);

    /// <summary>
    /// Поиск и удаление символа "/" и всего, что после него до конца строки.
    /// </summary>
    /// <param name="text">Тескт.</param>
    /// <returns>Результат.</returns>
    string RemoveAfterSlash(string? text);
}