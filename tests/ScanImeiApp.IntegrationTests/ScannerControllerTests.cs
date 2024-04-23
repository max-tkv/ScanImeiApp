using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using ScanImeiApp.Contracts.Models;
using Xunit;

namespace ScanImeiApp.IntegrationTests;

public class ScannerControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public ScannerControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task ScanAsync_ValidImages_ReturnsOkWithImeiResponse()
    {
        // Arrange
        var image1 = await GenerateImageAsync("image1.jpg", 1024);
        var image2 = await GenerateImageAsync("image2.jpg", 2048);
        var formData = new MultipartFormDataContent();
        formData.Add(new StreamContent(image1), "images", "image1.jpg");
        formData.Add(new StreamContent(image2), "images", "image2.jpg");

        // Act
        var response = await _client.PostAsync("/scan", formData);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        var imeiResponses = JsonSerializer.Deserialize<List<ImeiResponse>>(
            content, 
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        Assert.NotNull(imeiResponses);
        Assert.Equal(2, imeiResponses.Count);
        // Assert.Equal("123456789012345", imeiResponses[0].Imei);
        // Assert.Equal("123456789012345", imeiResponses[1].Imei);
    }

    private async Task<Stream> GenerateImageAsync(string fileName, long length)
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(new string('0', (int)length));
        writer.Flush();
        stream.Position = 0;
        return stream;
    }
}