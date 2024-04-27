using AutoFixture;
using Moq;
using ScanImeiApp.Abstractions;
using ScanImeiApp.Options;
using ScanImeiApp.Services.ModificationImage;
using Xunit;

namespace ScanImeiApp.UnitTests.Services.ModificationImage;

/// <summary>
/// Тесты для класса <see cref="ModifierImageAdjustContrast" />.
/// </summary>
public class ModifierImageAdjustContrastTest : BaseUnitTests
{
    [Fact(DisplayName = "Проверяет, что метод ModifyImageAsync вызывает метод AdjustContrastAsync с правильными параметрами.")]
    public async Task ModifyImageAsync_CallsAdjustContrastAsync_WithCorrectParameters()
    {
        // Arrange
        var mockImageService = new Mock<IImageService>();
        var appOptions = _fixture.Create<AppOptions>();
        var sut = new ModifierImageAdjustContrast(appOptions, mockImageService.Object);
        var memoryStreamImage = new MemoryStream();
        var imageName = "testImage.jpg";
        var cancellationToken = CancellationToken.None;
        
        mockImageService.Setup(service => service.AdjustContrastAsync(
                memoryStreamImage, 
                imageName, 
                appOptions.ImageOptions.Contrast, 
                cancellationToken))
            .ReturnsAsync(memoryStreamImage)
            .Verifiable("AdjustContrastAsync was not called with the correct parameters.");

        // Act
        await sut.ModifyImageAsync(memoryStreamImage, imageName, cancellationToken);

        // Assert
        mockImageService.Verify();
    }
}