using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ScanImeiApp.Models;
using Tesseract;
using TesseractOCR.Enums;
using EngineMode = Tesseract.EngineMode;
using Page = TesseractOCR.Page;

namespace ScanImeiApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Upload(List<IFormFile> images)
    {
        try
        {
            var result = new List<string>();
            foreach (var image in images)
            {
                if (image.Length <= 0)
                {
                    continue;
                };
                
                using var memoryStream = new MemoryStream();
                await image.CopyToAsync(memoryStream);

                using var engine = new TesseractEngine(@"tessdata", "eng", EngineMode.Default);
                using Pix img = Pix.LoadFromMemory(memoryStream.ToArray());
                using Tesseract.Page recognizedPage = engine.Process(img);
                string recognizedText = recognizedPage.GetText();
                Console.WriteLine("Text: \r\n{0}", recognizedText);
                        
                result.Add(image.FileName);
            }

            return Json(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading images");
            return BadRequest("Error uploading images");
        }
    }
    
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}