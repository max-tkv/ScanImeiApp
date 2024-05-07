using AutoFixture;
using Moq;
using ScanImeiApp.Abstractions;
using ScanImeiApp.Options;
using ScanImeiApp.Services.ModificationImage;
using Xunit;

namespace ScanImeiApp.UnitTests.Services.ModificationImage;

/// <summary>
/// Тесты для класса <see cref="ModifierImageAdjustSharpness" />.
/// </summary>
public class ModifierImageAdjustSharpnessTest : BaseUnitTests
{
    [Fact(DisplayName = "Проверяет, что метод ModifyImageAsync вызывает метод AdjustSharpnessAsync с правильными параметрами.")]
    public async Task ModifyImageAsync_CallsAdjustContrastAsync_WithCorrectParameters()
    {
        // Arrange
        var mockImageService = new Mock<IImageService>();
        var appOptions = _fixture.Create<AppOptions>();
        var modifier = new ModifierImageAdjustSharpness(appOptions, mockImageService.Object);
        var memoryStreamImage = new MemoryStream();
        var imageName = "testImage.jpg";
        var cancellationToken = CancellationToken.None;
        
        mockImageService.Setup(service => service.AdjustSharpnessAsync(
                memoryStreamImage, 
                imageName, 
                appOptions.ImageOptions!.Sharpness, 
                cancellationToken))
            .ReturnsAsync(memoryStreamImage)
            .Verifiable("AdjustSharpnessAsync was not called with the correct parameters.");

        // Act
        await modifier.ModifyImageAsync(memoryStreamImage, imageName, cancellationToken);

        // Assert
        mockImageService.Verify();
    }
}