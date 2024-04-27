using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ScanImeiApp.Contracts.Models;
using ScanImeiApp.Web.Controllers;
using Xunit;

namespace ScanImeiApp.UnitTests.Controllers;

/// <summary>
/// Тесты для контроллера <see cref="HomeController" />.
/// </summary>
public class HomeControllerTests : BaseUnitTests
{
    private readonly HomeController _controller;

    public HomeControllerTests()
    {
        _controller = new HomeController();
    }

    [Fact]
    public void Index_ReturnsViewResult()
    {
        // Act
        var result = _controller.Index();

        // Assert
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public void Error_ReturnsViewResult_WithCorrectModel()
    {
        // Arrange
        var activity = new Activity(string.Empty);
        activity.Start();

        // Act
        var result = _controller.Error() as ViewResult;

        // Assert
        Assert.IsType<ViewResult>(result);
        Assert.IsType<ErrorView>(result.Model);
        Assert.NotNull(result.Model);
    }
}