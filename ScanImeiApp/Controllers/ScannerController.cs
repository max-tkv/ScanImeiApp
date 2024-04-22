using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using ScanImeiApp.Abstractions;
using ScanImeiApp.Models;

namespace ScanImeiApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ScannerController : ControllerBase
{
    /// <summary>
    /// Выполнить сканирование изображений на наличие IMEI.
    /// </summary>
    /// <param name="scanImeiTextService"></param>
    /// <param name="images">Изображения.</param>
    /// <returns>Список найденных IMEI.</returns>
    [HttpPost("/scan")]
    public async Task<IActionResult> ScanAsync(
        [FromServices] IScanImeiTextService scanImeiTextService, 
        List<IFormFile> images)
    {
        var result = new List<ImeiResult>();
        foreach (var image in images.Where(image => image.Length > 0))
        {
            using var memoryStream = new MemoryStream();
            await image.CopyToAsync(memoryStream);

            var imeis = scanImeiTextService.GetImeiTextFromImage(memoryStream);
            result.Add(new ImeiResult
            {
                ImageName = image.FileName,
                Imei = imeis
            });
        }

        return Ok(JsonSerializer.Serialize(result));
    }
}