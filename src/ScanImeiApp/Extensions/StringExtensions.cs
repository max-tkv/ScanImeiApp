namespace ScanImeiApp.Extensions;

public static class StringExtensions
{
    private const char CoupletChar = ':';
    
    /// <summary>
    /// Удалить все двоеточие в начале строки.
    /// </summary>
    /// <param name="text">Текст.</param>
    /// <returns>Результат.</returns>
    public static string DeleteFirstColonsChar(this string text)
    {
        int removeCount = 0;
        foreach (var firstChar in text)
        {
            if (firstChar == CoupletChar)
            {
                removeCount++;
            }
            else
            {
                break;
            }
        }
        
        return text[removeCount..];
    }

    /// <summary>
    /// Удалить все двоеточие в конце строки.
    /// </summary>
    /// <param name="text">Текст.</param>
    /// <returns>Результат.</returns>
    public static string DeleteLastColonsChar(this string text)
    {
        int removeCount = 0;
        for (int i = text.Length - 1; i > 0; i--)
        {
            char lastChar = text[i];
            if (lastChar == CoupletChar)
            {
                removeCount++;
            }
            else
            {
                break;
            }
        }
        
        return text[..^removeCount];
    }
    
    /// <summary>
    /// Замены нескольких двоеточий подряд на одно двоеточие.
    /// </summary>
    /// <param name="text">Текст.</param>
    /// <returns>Результат.</returns>
    public static string ReplaceMultipleColons(this string text)
    {
        string result = string.Empty;
        bool wasColon = false;
        foreach (char c in text)
        {
            if (c == CoupletChar)
            {
                if (!wasColon)
                {
                    result += c;
                    wasColon = true;
                }
            }
            else
            {
                result += c;
                wasColon = false;
            }
        }

        return result;
    }
}