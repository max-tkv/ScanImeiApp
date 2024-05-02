using Moq;
using ScanImeiApp.Tesseract.Abstractions;
using ScanImeiApp.Tesseract.Services;
using Tesseract;
using Xunit;

namespace ScanImeiApp.UnitTests;

/// <summary>
/// Тесты для класса <see cref="TesseractService" />.
/// </summary>
public class TesseractServiceTests : BaseUnitTests
{
    private readonly Mock<ITesseractEngineAdapter> _mockEngineAdapter;
    private readonly Mock<ITesseractPixService> _mockPixService;
    private readonly TesseractService _tesseractService;
    private readonly Mock<ITesseractPageAdapter> _mockTesseractPage;

    public TesseractServiceTests()
    {
        _mockEngineAdapter = new Mock<ITesseractEngineAdapter>();
        _mockPixService = new Mock<ITesseractPixService>();
        _mockTesseractPage = new Mock<ITesseractPageAdapter>();
        _tesseractService = new TesseractService(_mockEngineAdapter.Object, _mockPixService.Object);
    }

    [Fact(DisplayName = "Проверяет, что метод Recognize возвращает правильный результат.")]
    public void Recognize_ShouldReturnRecognizeResult_WhenCalledWithValidMemoryStream()
    {
        // Arrange
        var memoryStreamImage = new MemoryStream();
        var expectedText = "expected text";
        var expectedConfidence = 0.85f;

        _mockPixService.Setup(s => s.LoadFromMemory(memoryStreamImage)).Returns(It.IsAny<Pix>());
        _mockEngineAdapter.Setup(e => e.Process(It.IsAny<Pix>(), PageSegMode.SingleColumn)).Returns(_mockTesseractPage.Object);
        _mockTesseractPage.Setup(p => p.GetMeanConfidence()).Returns(expectedConfidence);
        _mockTesseractPage.Setup(p => p.GetText()).Returns(expectedText);

        // Act
        var result = _tesseractService.Recognize(memoryStreamImage);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedConfidence, result.Confidence);
        Assert.Equal(expectedText, result.Text);
        
        _mockPixService
            .Verify(service => service.LoadFromMemory(memoryStreamImage), 
                Times.Once);
        
        _mockEngineAdapter
            .Verify(service => service.Process(It.IsAny<Pix>(), PageSegMode.SingleColumn), 
                Times.Once);
        
        _mockTesseractPage
            .Verify(service => service.GetMeanConfidence(), 
                Times.Once);
        
        _mockTesseractPage
            .Verify(service => service.GetText(), 
                Times.Once);
    }
}