using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using ScanImeiApp.Abstractions;

namespace ScanImeiApp.Services;

/// <summary>
/// Класс представляет функционал работы с регулярными выражениями.
/// </summary>
public class RegexService : IRegexService
{
    /// <inheritdoc />
    public async Task<List<string>> FindAndExtractedByPatternsAsync(
        string recognizedText, 
        IReadOnlyCollection<string> patterns,
        CancellationToken cancellationToken)
    { 
        List<Regex> compiledRegexes = CreateCompiledRegexes(patterns);
        return await ExtractFromTextParallelAsync(
            recognizedText, 
            compiledRegexes, 
            cancellationToken);
    }
    
    /// <inheritdoc />
    public string RemoveAfterSlash(string text) =>
        Regex.Replace(text, @"/.*$", "", RegexOptions.Multiline);

    #region Приватные методы

    /// <summary>
    /// Создать список предварительно скомпилированных регулярных выражений.
    /// </summary>
    /// <param name="patterns">Шаблоны.</param>
    /// <returns>Предварительно скомпилированные регулярные выражения.</returns>
    private static List<Regex> CreateCompiledRegexes(IReadOnlyCollection<string> patterns)
    {
        var compiledRegexes = new List<Regex>();
        foreach (string pattern in patterns)
        {
            compiledRegexes.Add(new Regex(pattern, RegexOptions.Compiled));
        }

        return compiledRegexes;
    }

    /// <summary>
    /// Выполнить поиск парлелльно в нескольких потоках.
    /// </summary>
    /// <param name="recognizedText">Текст для анализа.</param>
    /// <param name="compiledRegexes">Предварительно скомпилированные регулярные выражения.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Результат поиска.</returns>
    private async Task<List<string>> ExtractFromTextParallelAsync(
        string recognizedText, 
        List<Regex> compiledRegexes,
        CancellationToken cancellationToken)
    {
        var result = new ConcurrentBag<string>();
        await Parallel.ForEachAsync(compiledRegexes, cancellationToken, (regex, _) =>
        {
            {
                MatchCollection matches = regex.Matches(recognizedText);
                foreach (Match match in matches)
                {
                    if (match.Success)
                    {
                        for (int i = 1; i < match.Groups.Count; i++)
                        {
                            string value = match.Groups[i].Value;
                            if (!string.IsNullOrWhiteSpace(value))
                            {
                                result.Add(value);
                            }   
                        }
                    }
                }
            }
            return new ValueTask(Task.CompletedTask);
        });

        return result.Distinct().ToList();
    }

    #endregion
}