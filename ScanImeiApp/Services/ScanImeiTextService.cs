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
    private const string ImeiPattern = @"IMEI\s(\d{15})";
    
    /// <inheritdoc />
    public string GetImeiTextFromImage(MemoryStream memoryStreamImage)
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
    private static string ExtractImeiFromText(string recognizedText)
    {
        Regex regex = new Regex(ImeiPattern);
        Match match = regex.Match(recognizedText);
        if (match.Success)
        {
            return match.Groups[1].Value;
        }

        throw new NotFoundImeiException();
    }
}