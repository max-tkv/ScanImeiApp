using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ScanImeiApp.Contracts.Models;

namespace ScanImeiApp.Web.Controllers;

/// <summary>
/// Контроллер для загрузки UI приложения.
/// </summary>
public class HomeController : Controller
{
    /// <summary>
    /// Метод для загрузки основной страницы приложения.
    /// </summary>
    /// <returns></returns>
    public IActionResult Index()
    {
        return View();
    }

    /// <summary>
    /// Метод для формирования данных для страницы с описанием ошибки.
    /// </summary>
    /// <returns>Данных для страницы с описанием ошибки.</returns>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorView { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}