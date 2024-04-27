using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using ScanImeiApp.Abstractions;
using ScanImeiApp.Models;
using ScanImeiApp.Options;
using ScanImeiApp.Services;
using Xunit;

namespace ScanImeiApp.UnitTests.Services;

/// <summary>
/// Тесты для класса <see cref="ImeiService" />.
/// </summary>
public class ImeiServiceTests : BaseUnitTests
{
    private readonly Mock<ILogger<ImeiService>> _loggerMock;
    private readonly Mock<IRegexService> _regexServiceMock;
    private readonly ImeiService _imeiService;

    public ImeiServiceTests()
    {
        _loggerMock = new Mock<ILogger<ImeiService>>();
        _regexServiceMock = new Mock<IRegexService>();
        var appOptions = new AppOptions
        {
            Patterns = new List<string> { "pattern1", "pattern2" },
            RequiredTextImei = new List<string> { "requiredText1", "requiredText2" }
        };
        _imeiService = new ImeiService(
            _loggerMock.Object, 
            appOptions, 
            _regexServiceMock.Object);
    }

    [Fact(DisplayName = "Проверяет, что метод GetFromTextAsync возвращает список извлеченных IMEI.")]
    public async Task GetFromTextAsync_ReturnsExtractedImei()
    {
        // Arrange
        var recognizeResults = _fixture.Create<List<RecognizeResult>>();
        var cancellationToken = CancellationToken.None;
        var expectedImeiList = new List<string> { "123456789012345", "987654321098765" };
        _regexServiceMock
            .Setup(x => x.FindAndExtractedByPatternsAsync(
                It.IsAny<string>(), 
                It.IsAny<List<string>>(), 
                cancellationToken))
            .ReturnsAsync(expectedImeiList);
        _regexServiceMock
            .Setup(x => x.RemoveAfterSlash(
                It.IsAny<string>()))
            .Returns(_fixture.Create<string>());

        // Act
        var result = await _imeiService.FindImeiToRecognizeResultsAsync(recognizeResults, cancellationToken);

        // Assert
        Assert.Equal(expectedImeiList, result);
        _regexServiceMock
            .Verify(x => x.FindAndExtractedByPatternsAsync(
                It.IsAny<string>(), 
                It.IsAny<List<string>>(), 
                cancellationToken), 
                Times.Once);
        _regexServiceMock
            .Verify(x => x.RemoveAfterSlash(
                    It.IsAny<string>()), 
                Times.Once);
        LogVerify(_loggerMock, LogLevel.Information, Times.Exactly(2));
    }
}