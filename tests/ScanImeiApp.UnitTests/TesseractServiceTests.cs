using AutoFixture;
using Moq;
using ScanImeiApp.Models;
using ScanImeiApp.Tesseract;
using Tesseract;
using Xunit;

namespace ScanImeiApp.UnitTests;

/// <summary>
/// Тесты для класса <see cref="TesseractService" />.
/// </summary>
public class TesseractServiceTests : BaseUnitTests, IDisposable
{
    private readonly Mock<TesseractEngine> _tesseractEngineMock;
    private readonly TesseractService _tesseractService;
    private readonly Fixture _fixture;

    public TesseractServiceTests()
    {
        _tesseractEngineMock = new Mock<TesseractEngine>(
            MockBehavior.Loose,
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            EngineMode.Default,
            _fixture.Create<string>()); 
        _tesseractService = new TesseractService(_tesseractEngineMock.Object);
        _fixture = new Fixture();
    }

    [Fact(DisplayName = "Recognize должен возвращать правильный RecognizeResult", 
        Skip = "Не работает tesseract на моем MAC.")]
    public void Recognize_ReturnsCorrectRecognizeResult()
    {
        // Arrange
        var memoryStreamImage = _fixture.Create<MemoryStream>();
        var expectedRecognizeResult = _fixture.Create<RecognizeResult>();

        // Act
        var result = _tesseractService.Recognize(memoryStreamImage);
    }

    public void Dispose()
    {
        _tesseractService.Dispose();
    }
}