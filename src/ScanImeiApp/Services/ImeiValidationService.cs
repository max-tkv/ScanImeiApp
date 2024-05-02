using Microsoft.Extensions.Logging;
using ScanImeiApp.Abstractions;

namespace ScanImeiApp.Services;

/// <summary>
/// Класс представляет сервис для валидации IMEI.
/// </summary>
public class ImeiValidationService : IImeiValidationService
{
    private readonly ILogger<ImeiValidationService> _logger;

    public ImeiValidationService(ILogger<ImeiValidationService> logger)
    {
        _logger = logger;
    }
    
    /// <inheritdoc />
    public List<string> FilterValidated(List<string> imei)
    {
        List<string> result = new List<string>();
        foreach (var imeiItem in imei)
        {
            if (ValidateImei(imeiItem))
            {
                _logger.LogDebug($"IMEI: {imeiItem} - валидный.");
                result.Add(imeiItem);
                continue;
            }
            
            _logger.LogDebug($"IMEI: {imeiItem} - не валидный.");
        }

        return result;
    }

    #region Приватные методы

    /// <summary>
    /// Валидация IMEI (алгоритм Луна). 
    /// </summary>
    /// <param name="imei">IMEI.</param>
    /// <returns><b>True</b> - корректный, <b>False</b> - не корректный.</returns>
    private bool ValidateImei(string imei)
    {
        if (imei.Length != 15 || !imei.All(char.IsDigit))
        {
            return false;
        }

        int sum = 0;
        for (int i = 0; i < imei.Length; i++)
        {
            int digit = imei[i] - '0';
            if (i % 2 == 1)
            {
                digit *= 2;
                if (digit > 9)
                {
                    digit -= 9;
                }
            }
            sum += digit;
        }

        return sum % 10 == 0;
    }

    #endregion
}