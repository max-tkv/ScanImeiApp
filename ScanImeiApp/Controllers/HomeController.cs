using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ScanImeiApp.Abstractions;
using ScanImeiApp.Exceptions;
using ScanImeiApp.Models;

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
    public async Task<IActionResult> Upload(
        [FromServices] IScanImeiTextService scanImeiTextService, 
        List<IFormFile> images)
    {
        try
        {
            var result = new List<string>();
            foreach (var image in images.Where(image => image.Length > 0))
            {
                using var memoryStream = new MemoryStream();
                await image.CopyToAsync(memoryStream);

                var firstImei = scanImeiTextService.GetImeiTextFromImage(memoryStream);
                result.Add($"IMEI1:{firstImei}");

                // todo
                result.Add($"IMEI2:{firstImei}");
            }

            return Json(result);
        }
        catch (NotFoundImeiException notFoundImeiException)
        {
            _logger.LogWarning(notFoundImeiException, "Не удалось найти IMEI.");
            return BadRequest("Не удалось найти IMEI.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при обработки изображения.");
            return BadRequest("Ошибка при обработки изображения.");
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}