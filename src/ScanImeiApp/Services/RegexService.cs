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
    public List<string> FindAndExtractedByPatterns(string recognizedText, IReadOnlyCollection<string> patterns)
    { 
        List<Regex> compiledRegexes = CreateCompiledRegexes(patterns);
        return ExtractFromTextParallel(recognizedText, compiledRegexes);
    }

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
    /// <returns>Результат поиска.</returns>
    private static List<string> ExtractFromTextParallel(string recognizedText, List<Regex> compiledRegexes)
    {
        var result = new ConcurrentBag<string>();
        Parallel.ForEach(compiledRegexes, regex =>
        {
            MatchCollection matches = regex.Matches(recognizedText);
            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    result.Add(match.Groups[1].Value);
                }
            }
        });

        return result.Distinct().ToList();
    }

    #endregion
}