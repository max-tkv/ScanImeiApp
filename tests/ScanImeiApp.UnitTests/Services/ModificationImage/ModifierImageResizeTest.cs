using AutoFixture;
using Moq;
using ScanImeiApp.Abstractions;
using ScanImeiApp.Options;
using ScanImeiApp.Services.ModificationImage;
using Xunit;

namespace ScanImeiApp.UnitTests.Services.ModificationImage;

/// <summary>
/// Тесты для класса <see cref="ModifierImageResize" />.
/// </summary>
public class ModifierImageResizeTest : BaseUnitTests
{
    [Fact(DisplayName = "Проверяет, что метод ModifyImageAsync вызывает метод ResizeAsync с правильными параметрами.")]
    public async Task ModifyImageAsync_ResizeAsync_WithCorrectParameters()
    {
        // Arrange
        var mockImageService = new Mock<IImageService>();
        var appOptions = _fixture.Create<AppOptions>();
        var modifier = new ModifierImageResize(appOptions, mockImageService.Object);
        var memoryStreamImage = new MemoryStream();
        var imageName = "testImage.jpg";
        var cancellationToken = CancellationToken.None;
        
        mockImageService.Setup(service => service.ResizeAsync(
                memoryStreamImage, 
                imageName, 
                appOptions.ImageOptions.ResizeDpi, 
                cancellationToken))
            .ReturnsAsync(memoryStreamImage)
            .Verifiable("ResizeAsync was not called with the correct parameters.");

        // Act
        await modifier.ModifyImageAsync(memoryStreamImage, imageName, cancellationToken);

        // Assert
        mockImageService.Verify();
    }
}