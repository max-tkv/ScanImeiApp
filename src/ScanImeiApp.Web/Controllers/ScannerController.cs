using System.Net;
using Microsoft.AspNetCore.Mvc;
using ScanImeiApp.Abstractions;
using ScanImeiApp.Web.Filters;
using ScanImeiApp.Contracts.Models;

namespace ScanImeiApp.Web.Controllers;

/// <summary>
/// Контроллер методов API приложения.
/// </summary>
[HandleExceptionsFilter]
[Route("api/[controller]")]
[ApiController]
public class ScannerController : ControllerBase
{
    /// <summary>
    /// Выполнить сканирование списка изображений на наличие IMEI.
    /// </summary>
    /// <param name="scannerImeiService"></param>
    /// <param name="images">Изображения.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Список найденных IMEI.</returns>
    [HttpPost("scan")]
    [ProducesResponseType(typeof(List<ImeiResponse>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> ScanAsync(
        [FromServices] IScannerImeiService scannerImeiService, 
        List<IFormFile> images,
        CancellationToken cancellationToken)
    {
        var result = new List<ImeiResponse>();
        foreach (var image in images.Where(image => image.Length > 0))
        {
            using var memoryStream = new MemoryStream();
            await image.CopyToAsync(memoryStream, cancellationToken);
            var imei = await scannerImeiService.GetImeiTextFromImageAsync(
                memoryStream, 
                image.FileName, 
                cancellationToken);
            
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
    /// <param name="scannerImeiService"></param>
    /// <param name="image">Изображение.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Список найденных IMEI.</returns>
    [HttpPost("scan/single")]
    [ProducesResponseType(typeof(ImeiResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> ScanAsync(
        [FromServices] IScannerImeiService scannerImeiService, 
        IFormFile image,
        CancellationToken cancellationToken)
    {
        using var memoryStream = new MemoryStream();
        await image.CopyToAsync(memoryStream, cancellationToken);
        var imei = await scannerImeiService.GetImeiTextFromImageAsync(
            memoryStream, 
            image.FileName, 
            cancellationToken);

        return Ok(new ImeiResponse
        {
            ImageName = image.FileName,
            Imei = imei
        });
    }
}