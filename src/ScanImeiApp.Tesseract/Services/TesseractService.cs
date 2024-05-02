using ScanImeiApp.Abstractions;
using ScanImeiApp.Models;
using ScanImeiApp.Tesseract.Abstractions;
using Tesseract;

namespace ScanImeiApp.Tesseract.Services;

/// <summary>
/// Класс представляет возможность взаимодействия c Tesseract.
/// </summary>
public class TesseractService : ITesseractService
{
    private readonly ITesseractEngineAdapter _tesseractEngineAdapter;
    private readonly ITesseractPixService _tesseractPixService;

    public TesseractService(
        ITesseractEngineAdapter tesseractEngineAdapter, 
        ITesseractPixService tesseractPixService)
    {
        _tesseractEngineAdapter = tesseractEngineAdapter;
        _tesseractPixService = tesseractPixService;
    }

    /// <inheritdoc />
    public RecognizeResult Recognize(MemoryStream memoryStreamImage)
    {
        using Pix img = _tesseractPixService.LoadFromMemory(memoryStreamImage);
        using ITesseractPageAdapter recognizedPage = _tesseractEngineAdapter.Process(img, PageSegMode.SingleColumn);
        float recognizedConfidence = recognizedPage.GetMeanConfidence();
        string recognizedText = recognizedPage.GetText();
        return new RecognizeResult
        {
            Confidence = recognizedConfidence,
            Text = recognizedText
        };
    }
}