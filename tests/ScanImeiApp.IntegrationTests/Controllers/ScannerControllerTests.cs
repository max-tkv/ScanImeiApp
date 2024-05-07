using Microsoft.AspNetCore.Mvc.Testing;
using ScanImeiApp.Web;
using Xunit;

namespace ScanImeiApp.IntegrationTests.Controllers;

/// <summary>
/// todo: не работет tesseract на моем mac.
/// </summary>
public class ScannerControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    public ScannerControllerTests(WebApplicationFactory<Program> factory)
    {
    }

    [Fact(Skip = "Не работает tesseract на моем MAC.")]
    public void ScanAsync_ValidImages_Test()
    {
    }
}