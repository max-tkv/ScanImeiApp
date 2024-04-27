using AutoFixture;
using Moq;
using ScanImeiApp.Abstractions;
using ScanImeiApp.Options;
using ScanImeiApp.Services.ModificationImage;
using Xunit;

namespace ScanImeiApp.UnitTests.Services.ModificationImage;

/// <summary>
/// Тесты для класса <see cref="ModifierImageGaussianBlur" />.
/// </summary>
public class ModifierImageGaussianBlurTest : BaseUnitTests
{
    [Fact(DisplayName = "Проверяет, что метод ModifyImageAsync вызывает метод GaussianBlurAsync с правильными параметрами.")]
    public async Task ModifyImageAsync_GaussianBlurAsync_WithCorrectParameters()
    {
        // Arrange
        var mockImageService = new Mock<IImageService>();
        var appOptions = _fixture.Create<AppOptions>();
        var modifier = new ModifierImageGaussianBlur(appOptions, mockImageService.Object);
        var memoryStreamImage = new MemoryStream();
        var imageName = "testImage.jpg";
        var cancellationToken = CancellationToken.None;
        
        mockImageService.Setup(service => service.GaussianBlurAsync(
                memoryStreamImage, 
                imageName, 
                appOptions.ImageOptions.GaussianBlur, 
                cancellationToken))
            .ReturnsAsync(memoryStreamImage)
            .Verifiable("GaussianBlurAsync was not called with the correct parameters.");

        // Act
        await modifier.ModifyImageAsync(memoryStreamImage, imageName, cancellationToken);

        // Assert
        mockImageService.Verify();
    }
}