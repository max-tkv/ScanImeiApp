using System.Net;
using Microsoft.AspNetCore.Mvc;
using ScanImeiApp.Abstractions;
using ScanImeiApp.Filters;
using ScanImeiApp.Models;

namespace ScanImeiApp.Controllers;

[HandleExceptionsFilter]
[ApiController]
[Route("api/[controller]")]
public class ScannerController : ControllerBase
{
    /// <summary>
    /// Выполнить сканирование списка изображений на наличие IMEI.
    /// </summary>
    /// <param name="scanImeiTextService"></param>
    /// <param name="images">Изображения.</param>
    /// <returns>Список найденных IMEI.</returns>
    [HttpPost("/scan")]
    [ProducesResponseType(typeof(List<ImeiResponse>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> ScanAsync(
        [FromServices] IScanImeiTextService scanImeiTextService, 
        List<IFormFile> images)
    {
        var result = new List<ImeiResponse>();
        foreach (var image in images.Where(image => image.Length > 0))
        {
            using var memoryStream = new MemoryStream();
            await image.CopyToAsync(memoryStream);

            var imei = scanImeiTextService.GetImeiTextFromImage(memoryStream, image.FileName);
            result.Add(new ImeiResponse
            {
                ImageName = image.FileName,
                Imei = imei
            });
        }

        return Ok(result);
    }
    
    /// <summary>
    /// Выполнить сканирование одного изображения на наличие IMEI.
    /// </summary>
    /// <param name="scanImeiTextService"></param>
    /// <param name="image">Изображение.</param>
    /// <returns>Список найденных IMEI.</returns>
    [HttpPost("/scan/single")]
    [ProducesResponseType(typeof(ImeiResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> ScanAsync(
        [FromServices] IScanImeiTextService scanImeiTextService, 
        IFormFile image)
    {
        using var memoryStream = new MemoryStream();
        await image.CopyToAsync(memoryStream);

        var imei = scanImeiTextService.GetImeiTextFromImage(memoryStream, image.FileName);

        return Ok(new ImeiResponse
        {
            ImageName = image.FileName,
            Imei = imei
        });
    }
}