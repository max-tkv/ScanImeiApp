using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ScanImeiApp.Contracts.Models;

namespace ScanImeiApp.Web.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorView { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}