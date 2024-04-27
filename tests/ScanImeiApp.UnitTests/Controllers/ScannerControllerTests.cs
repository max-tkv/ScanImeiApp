using AutoFixture;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ScanImeiApp.Abstractions;
using ScanImeiApp.Contracts.Models;
using ScanImeiApp.Web.Controllers;
using Xunit;

namespace ScanImeiApp.UnitTests.Controllers;

/// <summary>
/// Тесты для контроллера <see cref="ScannerController" />.
/// </summary>
public class ScannerControllerTests : BaseUnitTests
{
    private readonly Mock<IScannerImeiService> _scannerImeiServiceMock;
    private readonly ScannerController _controller;

    public ScannerControllerTests()
    {
        _scannerImeiServiceMock = new Mock<IScannerImeiService>();
        _controller = new ScannerController();
    }

    [Fact(DisplayName = "Проверяет, что метод c множеством входных изображений " +
                        "возвращают OkObjectResult с правильными данными ImeiResponse.")]
    public async Task ScanAsync_MultipleImages_ReturnsOkResultWithImeiResponses()
    {
        // Arrange
        var imageName = _fixture.Create<string>();
        List<IFormFile> images = new List<IFormFile>
        {
            CreateIFormFileMock(string.Empty, imageName).Object,
            CreateIFormFileMock(string.Empty, imageName).Object,
            CreateIFormFileMock(string.Empty, imageName).Object
        };
        var listImeiResult = _fixture
            .CreateMany<string>(images.Count)
            .ToList();
        _scannerImeiServiceMock
            .Setup(service => service.GetImeiTextFromImageAsync(
                It.IsAny<MemoryStream>(), 
                It.IsAny<string>(), 
                CancellationToken.None))
            .ReturnsAsync(listImeiResult);

        // Act
        var result = await _controller.ScanAsync(
            _scannerImeiServiceMock.Object, 
            images,
            CancellationToken.None);

        // Assert
        _scannerImeiServiceMock
            .Verify(x => x.GetImeiTextFromImageAsync(
                    It.IsAny<MemoryStream>(), 
                    It.IsAny<string>(), 
                    CancellationToken.None), 
                Times.Exactly(images.Count));
        var okResult = Assert.IsType<OkObjectResult>(result);
        var imeiResponses = Assert.IsType<List<ImeiResponse>>(okResult.Value);
        Assert.Equal(images.Count, imeiResponses.Count);
        
        Assert.Equal(imageName, imeiResponses[0].ImageName);
        Assert.Equal(listImeiResult[0], imeiResponses[0].Imei[0]);
        Assert.Equal(listImeiResult[1], imeiResponses[0].Imei[1]);
        Assert.Equal(listImeiResult[2], imeiResponses[0].Imei[2]);
        
        Assert.Equal(imageName, imeiResponses[1].ImageName);
        Assert.Equal(listImeiResult[0], imeiResponses[1].Imei[0]);
        Assert.Equal(listImeiResult[1], imeiResponses[1].Imei[1]);
        Assert.Equal(listImeiResult[2], imeiResponses[1].Imei[2]);
        
        Assert.Equal(imageName, imeiResponses[2].ImageName);
        Assert.Equal(listImeiResult[0], imeiResponses[2].Imei[0]);
        Assert.Equal(listImeiResult[1], imeiResponses[2].Imei[1]);
        Assert.Equal(listImeiResult[2], imeiResponses[2].Imei[2]);
    }

    [Fact(DisplayName = "Проверяет, что метод c одним входным изображением " +
                        "возвращают OkObjectResult с правильными данными ImeiResponse.")]
    public async Task ScanAsync_SingleImage_ReturnsOkResultWithImeiResponse()
    {
        // Arrange
        var expectedImei = _fixture.Create<string>();
        var listImeiResult = new List<string> { expectedImei };
        var imageName = _fixture.Create<string>();
        var image = CreateIFormFileMock(string.Empty, imageName);
        var cancellationToken = CancellationToken.None;
        _scannerImeiServiceMock
            .Setup(service => service.GetImeiTextFromImageAsync(
                It.IsAny<MemoryStream>(), 
                It.IsAny<string>(), 
                cancellationToken))
            .ReturnsAsync(listImeiResult);

        // Act
        var result = await _controller.ScanAsync(
            _scannerImeiServiceMock.Object, 
            image.Object, 
            cancellationToken);

        // Assert
        _scannerImeiServiceMock
            .Verify(x => x.GetImeiTextFromImageAsync(
                    It.IsAny<MemoryStream>(), 
                    It.IsAny<string>(), 
                    CancellationToken.None), 
                Times.Once);
        var okResult = Assert.IsType<OkObjectResult>(result);
        var imeiResponse = Assert.IsType<ImeiResponse>(okResult.Value);
        Assert.NotNull(imeiResponse);
        Assert.Equal(imageName, imeiResponse.ImageName);
        Assert.Equal(expectedImei, imeiResponse.Imei.First());
    }
}