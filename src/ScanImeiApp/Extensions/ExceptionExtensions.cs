using System.Text;

namespace ScanImeiApp.Extensions;

/// <summary>
/// Класс расширение для исключений.
/// </summary>
public static class ExceptionExtensions
{
    /// <summary>
    /// Получить расширенное и структрированное описание исключения.
    /// </summary>
    /// <param name="ex">Исключения.</param>
    /// <returns>Описание исключения.</returns>
    public static string GetExceptionMessage(this Exception ex)
    {
        var exceptionsMessageChain = new StringBuilder(ex.Message);
        while (ex.InnerException != null)
        {
            ex = ex.InnerException;
            exceptionsMessageChain.AppendLine("--->");
            exceptionsMessageChain.AppendLine(ex.Message);
        }
        if (!string.IsNullOrEmpty(ex.StackTrace))
        {
            exceptionsMessageChain.AppendLine(ex.StackTrace);
        }
        
        return exceptionsMessageChain.ToString();
    }
}