using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using ScanImeiApp.Abstractions;
using ScanImeiApp.Models;
using ScanImeiApp.Services;
using Xunit;

namespace ScanImeiApp.UnitTests.Services;

/// <summary>
/// Тесты для класса <see cref="RecognizerTextService" />.
/// </summary>
public class RecognizerTextServiceTests : BaseUnitTests
{
    private readonly Mock<ITesseractService> _tesseractServiceMock;
    private readonly Mock<ILogger<RecognizerTextService>> _loggerMock;
    private readonly RecognizerTextService _recognizerTextService;

    public RecognizerTextServiceTests()
    {
        _tesseractServiceMock = new Mock<ITesseractService>();
        _loggerMock = new Mock<ILogger<RecognizerTextService>>();
        _recognizerTextService = new RecognizerTextService(
            _tesseractServiceMock.Object, 
            _loggerMock.Object);
    }

    [Fact(DisplayName = "Проверяет, что метод RecognizeText возвращает ожидаемый результат " +
                        "и корректно устанавливает свойства.")]
    public void RecognizeText_ReturnsCorrectRecognizeResult()
    {
        // Arrange
        var memoryStreamImage = new MemoryStream();
        var imageName = "testImage.jpg";
        var recognizerName = "testRecognizer";
        var expectedRecognizeResult = _fixture.Create<RecognizeResult>();
        _tesseractServiceMock
            .Setup(x => x.Recognize(It.IsAny<MemoryStream>()))
            .Returns(expectedRecognizeResult);

        // Act
        RecognizeResult result = _recognizerTextService.RecognizeText(
            memoryStreamImage, 
            imageName, 
            recognizerName);

        // Assert
        Assert.Equal(expectedRecognizeResult, result);
        Assert.Equal(imageName, result.ImageName);
        Assert.Equal(recognizerName, result.RecognizerName);
        _tesseractServiceMock.Verify(x => x.Recognize(memoryStreamImage), Times.Once);
        LogVerify(
            _loggerMock, 
            LogLevel.Debug, 
            "Результат изъятия текста с изображения", 
            Times.Once());
    }
}