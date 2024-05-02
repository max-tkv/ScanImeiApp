using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using ScanImeiApp.Services;
using Xunit;

namespace ScanImeiApp.UnitTests.Services;

/// <summary>
/// Тесты для класса <see cref="ImeiValidationService" />.
/// </summary>
public class ImeiValidationServiceTests : BaseUnitTests
{
    private readonly Mock<ILogger<ImeiValidationService>> _loggerMock;
    private readonly ImeiValidationService _imeiValidationService;

    public ImeiValidationServiceTests()
    {
        _loggerMock = new Mock<ILogger<ImeiValidationService>>();
        _imeiValidationService = new ImeiValidationService(_loggerMock.Object);
    }

    [Fact(DisplayName = "Проверяет, что метод возвращает только валидные IMEI. Алгоритм Луна.")]
    public void FilterValidated_ReturnsOnlyValidImei()
    {
        // Arrange
        var imei = new List<string>
        {
            "490154203237518", // Valid
            "123456789012345", // Invalid
            _fixture.Create<string>() // Random
        };

        // Act
        var result = _imeiValidationService.FilterValidated(imei);

        // Assert
        Assert.Single(result);
        Assert.Contains("490154203237518", result);
    }
}