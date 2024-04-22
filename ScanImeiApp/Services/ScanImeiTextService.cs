using System.Text.RegularExpressions;
using ScanImeiApp.Abstractions;
using ScanImeiApp.Exceptions;
using Tesseract;

namespace ScanImeiApp.Services;

/// <summary>
/// Класс представляющий возможность сканирования изображения на наличие IMEI в виде текста.
/// </summary>
public class ScanImeiTextService : IScanImeiTextService
{
    private const string TesseractLanguageName = "eng";
    private const string TesseractTessdataPath = "tessdata";
    private static readonly string[] ImeiPatterns =
    {
        @"IMEI\s(\d{15})",
        @"IMEI:\s(\d{15})"
    };

    /// <inheritdoc />
    public List<string> GetImeiTextFromImage(MemoryStream memoryStreamImage)
    {
        var recognizedText = RecognizedTextFromImage(memoryStreamImage);
        return ExtractImeiFromText(recognizedText);
    }

    /// <summary>
    /// Получить текст с изображения.
    /// </summary>
    /// <param name="memoryStreamImage">Изображение.</param>
    /// <returns>Текст.</returns>
    private static string RecognizedTextFromImage(MemoryStream memoryStreamImage)
    {
        using var engine = new TesseractEngine(
            TesseractTessdataPath, 
            TesseractLanguageName, 
            EngineMode.Default);
        using Pix img = Pix.LoadFromMemory(memoryStreamImage.ToArray());
        using Page recognizedPage = engine.Process(img);
        return recognizedPage.GetText();
    }

    /// <summary>
    /// Получить IMEI из текста используя регулярные выражения.
    /// </summary>
    /// <param name="recognizedText">Текст.</param>
    /// <returns>IMEI-строка</returns>
    /// <exception cref="NotFoundImeiException">Не удалось найти IMEI.</exception>
    private static List<string> ExtractImeiFromText(string recognizedText)
    {
        var result = new List<string>();
        foreach (var pattern in ImeiPatterns)
        {
            Regex regex = new Regex(pattern);
            Match match = regex.Match(recognizedText);
            if (match.Success)
            {
                result.Add(match.Groups[1].Value);
            }
        }

        if (result.Any())
        {
            return result;
        }
        
        throw new NotFoundImeiException();
    }
}